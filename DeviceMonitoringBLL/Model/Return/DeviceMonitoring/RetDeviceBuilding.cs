using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetDeviceBuilding
    {
       
        public long id { get; set; }
        public string mac { get; set; }

        public Nullable<System.DateTime> creattime { get; set; }
        public string IMEI { get; set; }
        public string tableType { get; set; }
        public string tableName { get; set; }
        public string tableId { get; set; }
    }
}
