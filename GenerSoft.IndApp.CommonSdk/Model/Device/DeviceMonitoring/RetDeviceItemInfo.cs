using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring
{
    public class RetDeviceItemInfo
    {
        public long ID { get; set; }
        public long DeviceModelID { get; set; }
        public string Name { get; set; }
        public string PropertyLabel { get; set; }
        public string ValueType { get; set; }
        public List<string> Unit { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public string OrgID { get; set; }
        public string State { get; set; }
    }
}
