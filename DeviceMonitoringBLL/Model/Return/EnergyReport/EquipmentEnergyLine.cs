using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.EnergyReport
{
    public class EquipmentEnergyLine
    {
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public List<List<object>> DeviceData { get; set; }
    }
}
