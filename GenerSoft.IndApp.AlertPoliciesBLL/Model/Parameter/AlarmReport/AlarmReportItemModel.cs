using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlarmReport
{
    public class AlarmReportItemModel
    {
        public string DeviceID { get; set; }
        public List<string> DeviceItemIDList { get; set; }
        public Nullable<DateTime> AlarmTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string OrgID { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
