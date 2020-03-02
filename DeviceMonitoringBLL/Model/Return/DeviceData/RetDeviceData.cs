using DeviceMonitoringBLL.Model.Return.DeviceMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceData
    {
        public string DeviceItemName { get; set; }
        public List<RetDeviceTableList> DeviceTableList { get; set; }
        public List<RetPieList> PieList { get; set; }
        public List<string> Time { get; set; }
        public List<double> Data { get; set; }
        public string Unit { get; set; }
        public List<RetDeviceInfo> DeviceList { get; set; }
    }
}
