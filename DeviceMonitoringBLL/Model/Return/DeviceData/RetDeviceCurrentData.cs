using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceCurrentData
    {
        public string ID { get; set; }
        public string DeviceId { get; set; }
        public string DeviceItemId { get; set; }
        public string DeviceItemName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string airRank { get; set; }
    }
}
