using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.HomeConfiguration;

namespace UserBLL.Model.Return.HomeConfiguration
{
    public class RetHomeConfiguration
    {
        public long ID { get; set; }
        public string DashBoardType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceItemID { get; set; }
        public string ChartName { get; set; }
        public string ChartType { get; set; }
        public string ChartConfig { get; set; }
        public string Remark { get; set; }
        public string DashBoardData { get; set; }
        public RetDashBoardDataList DashBoardDataList { get; set; }
        public Nullable<int> SortID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public string OrgID { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string XAxis { get; set; }
        public string YAxis { get; set; }
        public string DatabaseType { get; set; } //数据库连接类型："0"设备数据源；"1"本地mysql数据源
        public string ValueType { get; set; } // 2-1：设备总数；2-2：设备位置；2-3：报警总数
        public string DataType { get; set; } // 1：最近时段数据；2：时间段数据；3：实时数据；4：时间点数据
        public string RecentInterval { get; set; }
        public string RecentUnit { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string StatisticalInterval { get; set; }
        public string IntervalUnit { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }
        public List<HomeDeviceInfo> HomeDeviceInfoList { get; set; }
        // 首页
        public List<BaseModel> Property { get; set; } //级联数据
        // 设备
        public List<DeviceBaseModel> DeviceItemList { get; set; } //设备属性数据
    }
}
