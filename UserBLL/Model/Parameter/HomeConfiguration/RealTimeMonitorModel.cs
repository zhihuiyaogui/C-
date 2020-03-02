using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.HomeConfiguration
{
    public class RealTimeMonitorModel
    {
        public long ID { get; set; }
        public string DashBoardType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string ChartType { get; set; }
        public string Remark { get; set; }
        public string DashBoardData { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public string OrgID { get; set; }
        public string mainWidth { get; set; }
        public string mainHeight { get; set; }
        public string clickX { get; set; }
        public string clickY { get; set; }
        public string bgUrl { get; set; }
        public List<string> DelDeviceTags { get; set; }
        public List<string> DelTags { get; set; }
        public string GroupID { get; set; }
    }
}
