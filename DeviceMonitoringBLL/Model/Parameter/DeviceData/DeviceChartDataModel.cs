using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class DeviceChartDataModel
    {
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string ChartType { get; set; }
        public string DatabaseType { get; set; } //数据库连接类型："0"设备数据源；"1"mysql数据源
        public string ValueType { get; set; } // 2-1：设备总数；2-2：设备位置；2-3：报警总数
        public string DataType { get; set; } // 1：最近时段数据；2：时间段数据；3：实时数据；4：时间点数据
        public string RecentInterval { get; set; }
        public string RecentUnit { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public long OrgID { get; set; }
    }
}
