using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetEquipmentReport
    {
        public string Name { get; set; }
        public RetDeviceInfo DeviceInfo { get; set; }
        public string TotalRun { get; set; }
        public string TotalStop { get; set; }
        public double OpeningRate  { get; set; }
        public List<StateDataModel> StateDataList { get; set; }
    }

    public class StateDataModel
    {
        public string Name { get; set; }
        public List<long> value { get; set; }
        public ItemStyleModel itemStyle { get; set; }
    }

    public class ItemStyleModel
    {
        public ColorModel normal { get; set; }
    }

    public class ColorModel
    {
        public string color { get; set; }
    }
}
