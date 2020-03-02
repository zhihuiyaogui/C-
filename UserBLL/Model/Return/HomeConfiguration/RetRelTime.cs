using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.HomeConfiguration
{
    public class RetRelTime
    {
        public long ID { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string DeviceLabel { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceModelID { get; set; }
        public string ConnectType { get; set; }
        public string DataConnect { get; set; }
        public string DataConnectID { get; set; }
        public List<RetRelTimeTag> TagList { get; set; }
        public List<RetDeviceItems> DeviceItems { get; set; }

    }
}
