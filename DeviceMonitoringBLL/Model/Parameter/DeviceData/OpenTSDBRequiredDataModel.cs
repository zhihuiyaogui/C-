using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceData
{
    public class OpenTSDBRequiredDataModel
    {
        public string url { get; set; }
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public string DeviceItemPropertyLabel { get; set; }
        public string TagMap { get; set; }
        public string Interval { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string GetTogetherType { get; set; }// 聚合方式 1 平均 2 求和
        public bool IsInspur { get; set; } //是否是浪潮TSDB，如果是则需要登录
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string DataConnectID { get; set; }
    }
}
