using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetDeviceItemInfo
    {
        public string ID { get; set; }
        public string DeviceID { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string PropertyLabel { get; set; }
        public string ValueType { get; set; }
        public List<string> Unit { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public string OrgID { get; set; }
    }
}
