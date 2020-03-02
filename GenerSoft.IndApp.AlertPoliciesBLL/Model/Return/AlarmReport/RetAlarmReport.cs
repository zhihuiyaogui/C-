using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlarmReport
{
    public class RetAlarmReport
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string AlarmNumber { get; set; }
        public string TotalTime { get; set; }
        public List<string> AlertData { get; set; }
    }
}
