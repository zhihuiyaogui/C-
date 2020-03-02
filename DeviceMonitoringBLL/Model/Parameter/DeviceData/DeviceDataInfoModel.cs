using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class DeviceDataInfoModel
    {
        public string ID { get; set; } // 报警策略ID 
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string DataConnectID { get; set; }
        public string TagMap { get; set; }
    }
}
