using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class DeviceModelInfoModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ModelLabel { get; set; }
        public string Description { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string SortID { get; set; }
        public string OrgID { get; set; }
        public List<DeviceModelItemInfoModel> Domains { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
