using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.EnergyReport
{
    public class EquipmentEnergyModel
    {
        public string Type { get; set; }// 聚合方式 1 平均 2 求和
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public List<BaseModel> Property { get; set; }//新增能源报表，设备id+设备属性id
    }
}
