using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Return.User
{
    public class RetUserOrganiseInfo
    {
        //userinfo
        public long Id { get; set; }
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
        //organiseinfo
        public string OrganizeId { get; set; }
        public string OrganizeCode { get; set; }
        public string OrganizeName { get; set; }
        public string OrganizeProvince { get; set; }
        public string OrganizeCity { get; set; }
        public string OrganizeArea { get; set; }
        public string OrganizeAddressDetail { get; set; }
        public string OrganizeLogoUrl { get; set; }
        public string OrganizeContactName { get; set; }
        public string OrganizePhone { get; set; }
        public string OrganizeFixedPhone { get; set; }
        public string OrganizeFax { get; set; }
        public string OrganizeZipCode { get; set; }
        public string OrganizeEmail { get; set; }
        public string OrganizeState { get; set; }
        public string OrganizeTradeLevel1 { get; set; }
        public string OrganizeTradeLevel2 { get; set; }
        public string OrganizeLocationAddress { get; set; }
        public Nullable<System.DateTime> OrganizeRegistrationTime { get; set; }
        public Nullable<System.DateTime> OrganizeCreateTime { get; set; }
        public string OrganizeCreateUserId { get; set; }
        public Nullable<System.DateTime> OrganizeUpdateTime { get; set; }
        public string OrganizeUpdateUserId { get; set; }
        public string OrganizeSite { get; set; }
        public string OrganizeDescription { get; set; }
        public Nullable<System.DateTime> OrganizeValidFrom { get; set; }
        public Nullable<System.DateTime> OrganizeValidTo { get; set; }
    }
}
