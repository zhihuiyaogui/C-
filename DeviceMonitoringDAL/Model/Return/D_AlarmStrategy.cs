using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring

{
    public class D_AlarmStrategy
    {
        public long Id { get; set; }
        public string mac { get; set; }
       
        public string TempcValue { get; set; }
        public string HumValue { get; set; }
        public string Tag { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
