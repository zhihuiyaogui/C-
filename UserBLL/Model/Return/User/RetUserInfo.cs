using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.User
{
    public class RetUserInfo
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string LockState { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public Nullable<int> IsAdmin { get; set; }
        public Nullable<int> IsManageAdmin { get; set; }
        public string HeadImgUrl { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidTo { get; set; }

        
        public string mac { get; set; }

        public Nullable<System.DateTime> creattime { get; set; }
        public string IMEI { get; set; }
        public string tableType { get; set; }
        public string tableName { get; set; }
        public string tableId { get; set; }
    }
}
