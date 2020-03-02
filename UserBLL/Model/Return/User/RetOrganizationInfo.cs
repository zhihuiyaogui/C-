using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.User
{
    public class RetOrganizationInfo
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string AddressDetail { get; set; }
        public string LogoUrl { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string FixedPhone { get; set; }
        public string Fax { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string TradeLevel1 { get; set; }
        public string TradeLevel2 { get; set; }
        public string LocationAddress { get; set; }
        public Nullable<System.DateTime> RegistrationTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdateUserId { get; set; }
        public string Site { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidTo { get; set; }
        public List<RetUserInfo> UserInfoList { get; set; }
    }
}
