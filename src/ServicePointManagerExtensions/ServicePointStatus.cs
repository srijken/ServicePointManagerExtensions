﻿using System.Collections.Generic;
using System.Linq;

namespace ServicePointManagerExtensions
{
    public sealed class ServicePointStatus
    {
        public ServicePointStatus()
        {
            ServicePoints = new List<ServicePointStatusServicePoint>();
        }
        public int DefaultConnectionLimit { get; set; }
        public int ServicePointCount { get; set; }

        public int TotalConnectionsOverall
        {
            get { return ServicePoints.Sum(x => x.TotalConnections); }
        }
        public List<ServicePointStatusServicePoint> ServicePoints { get; }
    }
}
