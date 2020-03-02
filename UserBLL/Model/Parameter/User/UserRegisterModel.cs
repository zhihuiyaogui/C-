using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.User
{
    public class UserRegisterModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public string Type { get; set; }
        public string ContactPhone { get; set; }
        public string CompanyName { get; set; }
    }
}
