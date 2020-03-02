using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.Device.DeviceData
{
    public class RetDeviceCurrentData
    {
        public string ID { get; set; }// 报警策略id
        public string DeviceItemId { get; set; }
        public string DeviceItemName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
