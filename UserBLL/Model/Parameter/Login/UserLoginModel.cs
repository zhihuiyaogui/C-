using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.Login
{
    public class UserLoginModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int LoginType { get; set; }
        public string timestamp { get; set; }
    }
}
