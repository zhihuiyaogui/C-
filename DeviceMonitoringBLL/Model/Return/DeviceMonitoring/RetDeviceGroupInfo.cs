using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetDeviceGroupInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ParentGroupID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdateUserID { get; set; }
        public string OrgID { get; set; }
        public string Image { get; set; }
        public List<RetDeviceGroupInfo> children { get; set; }
    }
}
