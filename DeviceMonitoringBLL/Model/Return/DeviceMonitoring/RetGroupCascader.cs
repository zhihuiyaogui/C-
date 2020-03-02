using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetGroupCascader
    {
        public string value { get; set; }
        public string label { get; set; }
        public List<RetGroupCascader> children { get; set; }
    }
}
