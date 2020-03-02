using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class GetDeviceDataParameter
    {
        public string DeviceInfo { get; set; }
        public string TokenID { get; set; }
        public string Tid { get; set; }
    }
}
