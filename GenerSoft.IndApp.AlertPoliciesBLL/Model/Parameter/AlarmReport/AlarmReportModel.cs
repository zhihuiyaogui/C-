using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlarmReport
{
    public class AlarmReportModel
    {
        public string DeviceName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartNumber { get; set; }
        public string EndNumber { get; set; }
        public string OrgID { get; set; }
        public List<string> GroupIDList { get; set; } // 查询分组
    }
}
