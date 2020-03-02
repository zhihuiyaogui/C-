using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring
{
    public class RetDataConnectConfiguration
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ServerAddress { get; set; }
        public string ServerPort { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string DataBase { get; set; }
        public string ConnectString { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdateUserID { get; set; }
        public string OrgID { get; set; }
    }
}
