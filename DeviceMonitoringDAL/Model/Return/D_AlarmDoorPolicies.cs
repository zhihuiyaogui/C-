using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class D_AlarmDoorPolicies
    { 
    public long id { get; set; }
    public string mac { get; set; }
    public string tag { get; set; }
    public int Alarmtime { get; set; }
    public string IMEI { get; set; }
    public Nullable<System.DateTime> CreateTime { get; set; }
  }
}
