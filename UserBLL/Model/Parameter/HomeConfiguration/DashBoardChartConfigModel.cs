using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.HomeConfiguration
{
    public class DashBoardChartConfigModel
    {
        public string DatabaseType { get; set; }
        public string DataType { get; set; }
        public string RecentInterval { get; set; }
        public string RecentUnit { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string ValueType { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }
        public List<HomeDeviceInfo> HomeDeviceInfoList { get; set; } // 设备信息 设备id，设备属性id
        // 首页
        public string SelectionType { get; set; } //1：设备；2：总览
    }
}
