using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class EquipmentReportModel
    {
        public string Name { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<string> GroupIDList { get; set; } // 查询分组
    }
}
