using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class DeviceDataListModel2
    {
        public List<string> DeviceIDs { get; set; }
        public string DeviceItemPropertyLabel { get; set; }
        public List<string> DeviceItemIDs { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public string GetTogetherType { get; set; }// 聚合方式 1 平均 2 求和
    }
}
