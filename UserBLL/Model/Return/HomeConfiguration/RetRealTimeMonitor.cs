using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.HomeConfiguration
{
    public class RetRealTimeMonitor
    {
        public long ID { get; set; }
        public string DashBoardType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceItemName { get; set; }
        public string ChartType { get; set; }
        public string Remark { get; set; }
        public string DashBoardData { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public string OrgID { get; set; }
        public string DatabaseType { get; set; } //数据库连接类型："0"设备数据源；"1"mysql数据源
        public string ValueType { get; set; }
        public string Left { get; set; }
        public string Top { get; set; }
        public string Value { get; set; }
        public string DataConnectID { get; set; }
        public List<RetRelTimeTag> TagList { get; set; }
        public string GroupID { get; set; }
    }
}
