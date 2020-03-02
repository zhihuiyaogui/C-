using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class MedicineListInfoModel
    {
        public long id { get; set; }
        public string mac { get; set; }
        public string price { get; set; }

        public long code { get; set; }

        public string goodsName { get; set; }

        public int Count { get; set; }

        public Nullable<System.DateTime> creatTime { get; set; }

        public Nullable<System.DateTime> StartDateTime { get; set; }

        public Nullable<System.DateTime> EndDateTime { get; set; }

        public string img { get; set; }

        public string manuName { get; set; }
        public string spec { get; set; }
    }
}
