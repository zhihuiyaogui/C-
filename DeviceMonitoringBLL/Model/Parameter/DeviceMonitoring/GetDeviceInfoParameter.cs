using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    public class GetDeviceInfoParameter
    {
        public string TokenID { get; set; }
        public string Tid { get; set; }
        public string ID { get; set; } // 单个设备查询
        public string OrgID { get; set; } // 多个设备查询
        public string Status { get; set; } // 启用/禁用
        public string DeviceLabel { get; set; }
        public string Phone { get; set; }
    }
}
