using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetAlertPolicies
    {
        public long id { get; set; }
        public string mac { get; set; }
        public string MinTempc { get; set; }
        public string MaxTempc { get; set; }
        public string MinHum { get; set; }
        public string MaxHum { get; set; }
        public string IMEI { get; set; }
        public int Alarmtime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }


    }
}
