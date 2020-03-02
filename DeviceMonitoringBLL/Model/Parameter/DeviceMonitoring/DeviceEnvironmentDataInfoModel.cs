using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class DeviceEnvironmentDataInfoModel
    {
        public long id { get; set; }
        public string tempc { get; set; }
        public string hum { get; set; }
        public int status { get; set; }//开关门状态："0"关；"1"开
        public string mac { get; set; }
       // public string id{ get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
       

    }
}
