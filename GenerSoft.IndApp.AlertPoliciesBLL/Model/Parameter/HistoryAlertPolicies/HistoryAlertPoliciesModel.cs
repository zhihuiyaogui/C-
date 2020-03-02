using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.HistoryAlertPolicies
{
    public class HistoryAlertPoliciesModel
    {
        public long ID { get; set; }
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string StrategyID { get; set; }
        public string StrategyValue { get; set; }
        public string StrategyName { get; set; }
        public string Compare { get; set; }
        public string Value { get; set; }
        public Nullable<DateTime> AlarmTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string OrgID { get; set; }
        public List<string> DeviceItemIDList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
