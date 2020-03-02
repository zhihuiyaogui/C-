using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.EnergyReport
{
    public class RetEquipmentEnergyChart
    {
        public List<string> Time { get; set; }
        public List<string> LineLegend { get; set; }
        public List<string> PieLegend { get; set; }
        public List<EquipmentEnergyLine> LineData { get; set; }
        public List<EquipmentEnergyPie> PieList { get; set; }
    }
}
