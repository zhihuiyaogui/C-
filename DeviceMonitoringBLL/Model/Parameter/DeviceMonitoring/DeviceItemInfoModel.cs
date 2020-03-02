using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class DeviceItemInfoModel
    {
        public long ID { get; set; }
        public long DeviceID { get; set; }
        public string Name { get; set; }
        public string PropertyLabel { get; set; }
        public int ValueType { get; set; }
        public List<string> Unit { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public string OrgID { get; set; }
    }
}
