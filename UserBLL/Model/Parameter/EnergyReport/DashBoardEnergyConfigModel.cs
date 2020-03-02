using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.HomeConfiguration;

namespace UserBLL.Model.Parameter.EnergyReport
{
    public class DashBoardEnergyConfigModel
    {
        public string DatabaseType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public List<HomeDeviceInfo> HomeDeviceInfoList { get; set; } // 设备信息 设备id，设备属性id
        public string Type { get; set; } //1：平均；2：求和
    }
}
