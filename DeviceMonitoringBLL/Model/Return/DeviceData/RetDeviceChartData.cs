using DeviceMonitoringBLL.Model.Return.DeviceMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceChartData
    {
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public List<RetPieList> PieList { get; set; } // 饼图数据
        public List<string> Time { get; set; }
        public List<double> Data { get; set; }
        public string Unit { get; set; } // 单位
        public List<RetDeviceInfo> DeviceList { get; set; } // 设备位置：ValueType = "2-2"
        public List<string> Legend { get; set; } // 折线图、柱状图多属性图例
        public List<DeviceMultipleData> MultipleData { get; set; } // 折线图、柱状图多属性数据
    }

    public class DeviceMultipleData
    {
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public List<List<object>> DeviceData { get; set; }
    }
}
