using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.User
{
    public class UserMessageLoginModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int LoginType { get; set; }
        public string timestamp { get; set; }
        public int mobilePassword { get; set; }
    }
}
