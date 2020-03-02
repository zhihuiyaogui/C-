using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring
{
    public class GetDeviceInfoParameter
    {
        public string ID { get; set; } // 设备ID：单个设备查询

        public string DeviceLabel { get; set; }
        public string Status { get; set; } // 启用/禁用
        public string TokenId { get; set; }

        public string Phone { get; set; }

        public string OrgID { get; set; }

        public void SetPostParameter(WebApiPostParameter postParameter)
        {
            if (TokenId != null && TokenId != "")
            {
                postParameter.Content.Add("TokenID", TokenId.ToString());
            }
            else
            {
                throw new Exception("Need TokenId");
            }
        }
    }
}
