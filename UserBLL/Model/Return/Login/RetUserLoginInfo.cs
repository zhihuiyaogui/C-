using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.Login
{
    public class RetUserLoginInfo
    {


        public string HeadImgUrl { get; set; }
        public string UserId { get; set; }
        public string RealName { get; set; }
        public string TokenId { get; set; }
        public string Address { get; set; }
        public string BirthDay { get; set; }
        public DateTime CreationDate { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string Phone { get; set; }
        public string Iv { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string AccountId { get; set; }
        public string ContactPhone { get; set; }
        public string LocationHref { get; set; }
        public bool IsPlatformAdmin { get; set; }
        public DateTime TokenDisabledTime { get; set; }
        public long OrgID { get; set; }


    }
}
