using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring
{
    public class IoTHubConfigurationModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<DateTime> CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<DateTime> UpdateTime { get; set; }
        public string UpdateUserID { get; set; }
        public string OrgID { get; set; }
        public string Remark { get; set; }
    }
}
