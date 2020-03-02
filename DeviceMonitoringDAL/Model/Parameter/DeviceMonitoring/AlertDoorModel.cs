using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringDAL.Model.Parameter.DeviceMonitoring
{
    public class AlertDoorModel
    {
        public long id { get; set; }
        public string mac { get; set; }
        public string tag { get; set; }
        public int Alarmtime { get; set; }
        public string IMEI { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string tableId { get; set; }
        public string tableName { get; set; }


    }
}
