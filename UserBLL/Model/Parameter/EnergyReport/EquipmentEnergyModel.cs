using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.EnergyReport;

namespace UserBLL.Model.Parameter.EnergyReport
{
    public class EquipmentEnergyModel
    {
        public long ID { get; set; }
        public string DashBoardType { get; set; }
        public string ChartConfig { get; set; }
        public Nullable<int> SortID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public string OrgID { get; set; }
        public string DatabaseType { get; set; } //数据库连接类型："0"设备数据源；"1"本地mysql数据源
        public string Type { get; set; }// 聚合方式 1 平均 2 求和
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public List<BaseModel> Property { get; set; }//设备属性信息，设备id+设备属性id
    }
}
