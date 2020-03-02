using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.User
{
    public class RetVerificationCode
    {
        public string ImgBase64 { get; set; }
        public string VerificationCode { get; set; }
    }
}
