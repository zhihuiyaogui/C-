using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model
{
    class YanHuaPayload
    {
        public string type { get; set; }
        public  List<YanHuaPayloadVal> values { get; set; }
    }

    class YanHuaPayloadVal
    {
        public string data_type { get; set; }
        public string device_ip { get; set; }
        public string time { get; set; }
        public string function_code { get; set; }
        public string data { get; set; }
        public string name { get; set; }

        public string operationValue { get; set; }
    }
}
