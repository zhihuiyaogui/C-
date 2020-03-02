using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringDAL.Model.Parameter.DeviceMonitoring
{
    public class AlertInfoModel
    {
        public long Id { get; set; }
        public string mac { get; set; }
        public string StrategyName { get; set; }
        public string TempcValue { get; set; }
        public string HumValue { get; set; }
        public string Tag { get; set; }
        public string IMEI { get; set; }
        public string hum { get; set; }
        public string tempc { get; set; }

        public Nullable<System.DateTime> CreateTime { get; set; }

        public string tableId { get; set; }
        public string tableName { get; set; }

    }
}
