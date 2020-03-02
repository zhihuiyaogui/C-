using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies
{
    public class EnableDeviceParmeter
    {
        public string TokenID { get; set; }
        public string Tid { get; set; }

        public bool IsEnable { get; set; }

        public long DeviceId { get; set; }
    }
}
