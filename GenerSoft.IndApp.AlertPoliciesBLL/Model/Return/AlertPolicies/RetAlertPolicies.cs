using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlertPolicies
{
    public class RetAlertPolicies
    {
        public string ID { get; set; }
        public string StrategyName { get; set; }
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string DataConnectID { get; set; }
        public List<RetTagMap> TagList { get; set; }
        public string DeviceItemId { get; set; }
        public string DeviceItemName { get; set; }
        public string Compare { get; set; }
        public string Threshold { get; set; }
        public string CurrentData { get; set; }
        public string Remark { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateUserId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Interval { get; set; }
        public string Active { get; set; }
        public string OrgID { get; set; }
    }
}
