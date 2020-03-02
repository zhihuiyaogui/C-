using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class DeviceInfoModel
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string DeviceLabel { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceModelID { get; set; }
        public string ConnectType { get; set; }
        public string IoTHubID { get; set; }
        public string DataConnect { get; set; }
        public string DataConnectID { get; set; }
        public List<DeviceTagModel> TagList { get; set; }
        public List<DeviceItemInfoModel> DeviceItems { get; set; }
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
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string GroupID { get; set; } // 新增、更新分组
        public List<string> GroupIDList { get; set; } // 查询分组
    }
}
