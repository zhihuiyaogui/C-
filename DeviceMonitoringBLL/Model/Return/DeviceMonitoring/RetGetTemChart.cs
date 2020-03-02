using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetGetTemChart
    {
        public List<String> time { get; set; }
        public List<String> tempc { get; set; }
        public string mac { get; set; }
        public String msg { get; set; }
        
       
        public Nullable<System.DateTime> creattime { get; set; }

        

    }
}
