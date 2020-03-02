using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceRequiredData
    {
        public string DeviceName { get; set; }
        public string DeviceLabel { get; set; }
        public string TagMap { get; set; }
        public string DeviceItemName { get; set; }
        public string DeviceItemPropertyLabel { get; set; }
        public string DataConnectID { get; set; }
        public string DataConnectType { get; set; }
        public string DataConnectServerAddress { get; set; }
        public string DataConnectServerPort { get; set; }
        public string DataConnectUserName { get; set; }
        public string DataConnectPassWord { get; set; }
        public string DataConnectDataBase { get; set; }
        public string Unit { get; set; }
    }
}
