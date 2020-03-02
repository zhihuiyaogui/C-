using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlarmReport
{
    public class RetAlarmReportDetail
    {
        public List<RetAlarmReportItem> AlarmReportItem { get; set; }
        public List<RetChartInfo> ChartInfo { get; set; }
    }
}
