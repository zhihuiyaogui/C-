using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetDeviceInfo
    {
        public string ID { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string DeviceLabel { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceModelID { get; set; }
        public List<RetDeviceItemInfo> DeviceItems { get; set; }
        public string ConnectType { get; set; }
        public string IoTHub { get; set; }
        public string IoTHubID { get; set; }
        public string DataConnect { get; set; }
        public string DataConnectID { get; set; }
        public List<RetDeviceTag> TagList { get; set; }
        public string Specification { get; set; }
        public string GPS { get; set; }
        public string Phone { get; set; }
        public string Manufacturer { get; set; }
        public string RunningState { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdateUserID { get; set; }
        public string OrgID { get; set; }
        public string GroupID { get; set; }
        public List<string> GroupIDList { get; set; }
    }
}
