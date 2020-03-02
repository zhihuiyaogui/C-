using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceData
{
    public class RetDeviceTableData
    {
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public List<RetDeviceTableList> DeviceTableList { get; set; }
        public List<string> Time { get; set; }
        public List<double> Data { get; set; }
        public string Unit { get; set; }
        public List<List<object>> AllCoordination { get; set; }//添加[[,],[,]]格式的变量，所有点，暂时没用到
        public List<List<object>> AnomCoordination { get; set; }//添加[[,],[,]]格式的变量，异常点，暂时没用到
        public List<List<object>> NormCoordination { get; set; }//添加[[,],[,]]格式的变量，正常点，暂时没用到
        public string type { get; set; }//运行报表界面，用来区别是列表数据还是图表数据，1 列表数据 2 图表数据
    }
}
