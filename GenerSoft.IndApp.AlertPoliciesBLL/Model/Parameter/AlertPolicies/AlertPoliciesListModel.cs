using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies
{
    public class AlertPoliciesListModel
    {
        public long ID { get; set; }
        public string StrategyName { get; set; }
        public string DeviceName { get; set; }

        //修改的 按照设备模板添加2018.11.12
        public string DeviceModelID { get; set; }
        //list存的是deviceID
        public List<string> DeviceList { get; set; }

        public string DeviceItemName { get; set; }
        public string Remark { get; set; }
        public Nullable<long> CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<long> UpdateUserId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Interval { get; set; }
        public string Active { get; set; }
        public string OrgID { get; set; }
        public List<AlertPropertyModel> Property { get; set; }
    }
}
