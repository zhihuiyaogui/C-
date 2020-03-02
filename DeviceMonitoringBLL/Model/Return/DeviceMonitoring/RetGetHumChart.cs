using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetGetHumChart
    {
        public List<String> time { get; set; }
        public List<String> hum { get; set; }
        public List<int> status { get; set; }

        public String msg { get; set; }


        public Nullable<System.DateTime> creattime { get; set; }
    }
    }
