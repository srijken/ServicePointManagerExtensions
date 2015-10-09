using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;

namespace ServicePointExtensions
{
    public class ServicePointMonitor
    {
        private readonly Action<ServicePointStatus> _status;

        private ServicePointMonitor(Action<ServicePointStatus> status)
        {
            _status = status;
        }

        private void Print()
        {
            var servicePoints = ListServicePoints();

            var status = new ServicePointStatus
                         {
                             DefaultConnectionLimit = ServicePointManager.DefaultConnectionLimit,
                             ServicePointCount = servicePoints.Count
                         };

            foreach (var serviceEndpoint in servicePoints)
            {
                status.ServicePoints.Add(PrintServicePointConnections(serviceEndpoint));
            }

            _status(status);
        }

        private List<ServicePoint> ListServicePoints()
        {
            var tableField = typeof(ServicePointManager).GetField("s_ServicePointTable", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var table = (Hashtable)tableField.GetValue(null);
            var keys = table.Keys.Cast<object>().ToList();


            return (from key in keys
                   select ((WeakReference)table[key]) into val
                   select val?.Target into target
                   where target != null
                   select target as ServicePoint).ToList();
        }

        private ServicePointStatusServicePoint PrintServicePointConnections(ServicePoint sp)
        {
            var spType = sp.GetType();
            var privateOrPublicInstanceField = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var connectionGroupField = spType.GetField("m_ConnectionGroupList", privateOrPublicInstanceField);
            var value = (Hashtable)connectionGroupField.GetValue(sp);
            var connectionGroups = value.Keys.Cast<object>().ToList();

            var servicePointStatus = new ServicePointStatusServicePoint
                                     {
                                         Address = sp.Address,
                                         ConnectionLimit = sp.ConnectionLimit,
                                         CurrentConnection = sp.CurrentConnections,
                                         ConnectionGroupCount = connectionGroups.Count,
                                         TotalConnections = 0
                                     };

            foreach (var key in connectionGroups)
            {
                var connectionGroup = value[key];
                var groupType = connectionGroup.GetType();
                var listField = groupType.GetField("m_ConnectionList", privateOrPublicInstanceField);
                var listValue = (ArrayList)listField.GetValue(connectionGroup);
                //Console.WriteLine("{3} {0}\nConnectionGroup: {1} Count: {2}",sp.Address, key,listValue.Count, DateTime.Now);

                servicePointStatus.ConnectionGroups.Add(new ServicePointStatusConnectionGroup
                                                        {
                                                            Key = key,
                                                            Count = listValue.Count
                                                        });

                servicePointStatus.TotalConnections += listValue.Count;
            }

            return servicePointStatus;
        }

        public static void Start(TimeSpan interval, Action<ServicePointStatus> output)
        {
            var thread = new Thread(() => {
                                              while (true)
                                              {
                                                  var monitor = new ServicePointMonitor(output);
                                                  monitor.Print();


                                                  Thread.Sleep(interval);
                                              }
            });

            thread.IsBackground = true;
            thread.Start();
        }
    }
}