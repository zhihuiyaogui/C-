using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies
{
    public class AlertPropertyModel
    {
        public string DeviceID { get; set; }
        public string DataConnectID { get; set; }
        public string DeviceItemId { get; set; }
        public string Compare { get; set; }
        public string Threshold { get; set; }
        public List<TagMapModel> TagList { get; set; }
    }
}
