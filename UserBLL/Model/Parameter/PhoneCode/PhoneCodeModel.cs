using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.PhoneCode
{
    public class PhoneCodeModel
    {
        public string Id { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ClientIp { get; set; }
        public string SmsType { get; set; }
        public string SmsContent { get; set; }
    }
}
