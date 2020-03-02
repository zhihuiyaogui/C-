using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceTableList
    {
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public string Data { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
    }
}
