using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies
{
    public class AlertPoliciesModel
    {
        public long ID { get; set; }
        public string StrategyName { get; set; }
        public string DeviceID { get; set; }
        public string DataConnectID { get; set; }
        public List<TagMapModel> TagList { get; set; }
        public string DeviceItemId { get; set; }
        public string Compare { get; set; }
        public string Threshold { get; set; }
        public string Remark { get; set; }
        public Nullable<long> CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<long> UpdateUserId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Interval { get; set; }
        public string Active { get; set; }
        public string OrgID { get; set; }
        public List<string> DeviceItemIDList { get; set; }
    }
}
