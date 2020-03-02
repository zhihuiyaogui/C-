using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetEnvironmentInfo
    {
        public string tempc { get; set; }
        public string hum { get; set; }
        public int status { get; set; }//开关门状态："0"关；"1"开
        public string mac { get; set; }
    }
}
