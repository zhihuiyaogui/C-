using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.User
{
    public class UserInfoModel
    {
        public long Id { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string LockState { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string NewContactPhone { get; set; }
        public Nullable<int> IsAdmin { get; set; }
        public Nullable<int> IsManageAdmin { get; set; }
        public string HeadImgUrl { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidTo { get; set; }
        
       
    }
}
