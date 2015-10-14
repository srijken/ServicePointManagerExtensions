using System;
using System.Collections.Generic;

namespace ServicePointManagerExtensions
{
    public sealed class ServicePointStatusServicePoint
    {
        public ServicePointStatusServicePoint()
        {
            ConnectionGroups = new List<ServicePointStatusConnectionGroup>();
        }

        public Uri Address { get; set; }
        public int ConnectionLimit { get; set; }
        public int CurrentConnection { get; set; }
        public int ConnectionGroupCount { get; set; }
        public int TotalConnections { get; set; }
        public List<ServicePointStatusConnectionGroup> ConnectionGroups { get; }
    }
}