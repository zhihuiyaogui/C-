using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class MySqlRequiredDataModel
    {
        public string DataConnectDataBase { get; set; }
        public string DataConnectServerAddress { get; set; }
        public string DataConnectServerPort { get; set; }
        public string DataConnectUserName { get; set; }
        public string DataConnectPassWord { get; set; }
        public string DeviceLabel { get; set; }
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public string DeviceItemPropertyLabel { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
