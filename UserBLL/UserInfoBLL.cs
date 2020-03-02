using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.User;
using UserBLL.Model.Return.User;
using UserDAL;
using UserDAL.Model.Parameter.User;
using UserDAL.Model.Return.User;

namespace UserBLL
{
    public class UserInfoBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 1.给手机号码发送验证短信
        /// </summary>
        /// <param name="mobile">待发送的手机号码</param>
        /// <param name="mobile_code">发送的验证码</param>
        /// <returns></returns>
        
        public static string PostUrl = ConfigurationManager.AppSettings["WebReference.Service.PostUrl"];
        public ReturnItem<List<UserDAL.U_PhoneCode>> SendSMS(SendMessageSMS parameter) {

            ReturnItem<List<UserDAL.U_PhoneCode>> r = new ReturnItem<List<UserDAL.U_PhoneCode>>();
            List<UserDAL.U_PhoneCode> listinfo = new List<UserDAL.U_PhoneCode>();
            using (UserEntities user = new UserEntities())
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.mobile).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "用户不存在";
                        return r;
                    }
                    else
                    {

                        string account = "C93722756";//C93722756用户名是登录用户中心->验证码、通知短信->帐户及签名设置->APIID//C0628409
                        string password = "e0c83285e9b0d8c4265d5314b33f8cff"; //e0c83285e9b0d8c4265d5314b33f8cff密码是请登录用户中心->验证码、通知短信->帐户及签名设置->APIKEY
                        Random rad = new Random();
                        int mobile_code = rad.Next(1000, 10000);
                       
                        string content = "您的验证码是：" + mobile_code + " 。请不要把验证码泄露给其他人。";

                        string postStrTpl = "account={0}&password={1}&mobile={2}&content={3}";

                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] postData = encoding.GetBytes(string.Format(postStrTpl, account, password, parameter.mobile, content));

                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
                        myRequest.Method = "POST";
                        myRequest.ContentType = "application/x-www-form-urlencoded";
                        myRequest.ContentLength = postData.Length;

                        Stream newStream = myRequest.GetRequestStream();

                        newStream.Write(postData, 0, postData.Length);
                        newStream.Flush();
                        newStream.Close();

                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                        if (myResponse.StatusCode == HttpStatusCode.OK)
                        {

                            //新增用户表
                            UserDAL.U_PhoneCode newuser = new UserDAL.U_PhoneCode()
                            {
                                Phone = parameter.mobile,
                                Code = mobile_code.ToString(),
                                StartTime = DateTime.Now,

                            };
                            user.U_PhoneCode.Add(newuser);
                            user.SaveChanges();
                            listinfo.Add(newuser);

                            if (listinfo.FirstOrDefault() == null)
                            {
                                r.Data = null;
                                r.Code = -1;
                                r.Msg = "验证码发送失败";
                            }
                            else
                            {
                                r.Data = listinfo;
                                r.Msg = "验证码发送成功";
                                r.Code = 0;
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            return r;
        }
        // <summary>
        /// 2.手机验证码登录验证
        /// </summary>
        public ReturnItem<RetUserMessageLoginModel> UserMessageLogin(UserMessageLoginModel parameter)
        {
            ReturnItem<RetUserMessageLoginModel> r = new ReturnItem<RetUserMessageLoginModel>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getinfo = user.U_PhoneCode.Where(u => u.Phone == parameter.UserName).OrderByDescending(s => s.StartTime).FirstOrDefault();
                    log.Info(getinfo.Code);

                    if (!getinfo.Code.Equals(parameter.PassWord))
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "验证码登录失败";
                        return r;
                    }
                   else
                    {
                      var getuserinfo = user.U_User.Where(u => u.AccountId == parameter.UserName).FirstOrDefault();
                        r.Data = new RetUserMessageLoginModel()
                        {
                            HeadImgUrl = getuserinfo.HeadImgUrl,
                            UserId = getuserinfo.ID.ToString(),
                            RealName = getuserinfo.Name,     
                            ContactPhone = getuserinfo.ContactPhone,                          
                            AccountId = getuserinfo.AccountId,
                           
                        };
                        r.Msg = "用户登录成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }



        /// <summary>
        /// 注册用户
        /// </summary>
        public ReturnItem<RetUserInfo> Register(UserRegisterModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.ContactPhone).FirstOrDefault();
                    if (getuserinfo != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "用户已存在";
                        return r;
                    }
                    if (getuserinfo == null)
                    {
                        //新增用户表
                        U_User newuser = new U_User()
                        {
                            AccountId = parameter.AccountId,
                            Name = parameter.Name,
                            ContactPhone = parameter.ContactPhone,
                           // PassWord = CommonTool.MD5(parameter.PassWord.Trim()),
                            PassWord= parameter.PassWord,
                            Type = "2", //组织（必须有组织id）
                            Status = "0",//0 启用 1 禁用
                            LockState = "1",//0 已审核 1 未审核
                            IsAdmin = 1,
                            IsManageAdmin = 0,
                            CreateTime = DateTime.Now
                        };
                        user.U_User.Add(newuser);
                        user.SaveChanges();

                        //新增组织表
                        string num = "";
                        System.Random Random = new System.Random();
                        num = Random.Next(0, 99999999).ToString();
                        U_Organization neworganization = new U_Organization()
                       {
                            Code = num,
                           Name = parameter.CompanyName == null ? "" : parameter.CompanyName,
                            State = "0",
                            CreateTime = DateTime.Now
                        };
                        user.U_Organization.Add(neworganization);
                        user.SaveChanges();
                        //新增用户组织关联表
                        var getinfo_1 = user.U_Organization.OrderByDescending(m => m.CreateTime).FirstOrDefault();
                        var getinfo_2 = user.U_User.Where(n => n.AccountId == parameter.ContactPhone).FirstOrDefault();
                        U_UserOrganizationRel newrel = new U_UserOrganizationRel()
                        {
                            UserID = getinfo_2.ID,
                            OrgID = getinfo_1.ID,
                            CreateTime = DateTime.Now
                        };
                        user.U_UserOrganizationRel.Add(newrel);
                        user.SaveChanges();

                        r.Msg = "用户信息注册成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 查询手机号是否存在
        /// </summary>
        public ReturnItem<RetUserInfo> CheckUserInfoByAccountId(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.ContactPhone).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        r.Msg = "用户信息获取成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 利用ID获取用户信息
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetUserInfo> GetUserInfoById(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        r.Msg = "用户信息获取成功";
                        r.Code = 0;
                        r.Data = new RetUserInfo()
                        {
                            Id = getuserinfo.ID.ToString(),
                            AccountId = getuserinfo.AccountId.ToString(),
                            Name = getuserinfo.Name.ToString(),
                            Type = getuserinfo.Type,
                            Status = getuserinfo.Status,
                            LockState = getuserinfo.LockState,
                            Email = getuserinfo.Email == null ? "" : getuserinfo.Email.ToString(),
                            ContactPhone = getuserinfo.ContactPhone.ToString(),
                            PassWord = getuserinfo.PassWord.ToString(),
                            IsAdmin = getuserinfo.IsAdmin,
                            IsManageAdmin = getuserinfo.IsManageAdmin,
                            HeadImgUrl = getuserinfo.HeadImgUrl == null ? "" : getuserinfo.HeadImgUrl.ToString(),
                            CreateTime = getuserinfo.CreateTime,
                            UpdateTime = getuserinfo.UpdateTime,
                            LastLoginDate = getuserinfo.LastLoginDate
                        };
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 利用AccountId更新用户信息
        /// </summary>
        public ReturnItem<RetUserInfo> UpdateUserInfoByAccountId(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.ContactPhone).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (parameter.AccountId != null && parameter.AccountId != "")
                        {
                            getuserinfo.AccountId = parameter.AccountId;
                        }
                        if (parameter.Name != null && parameter.Name != "")
                        {
                            getuserinfo.Name = parameter.Name;
                        }
                        if (parameter.PassWord != null && parameter.PassWord != "" && parameter.PassWord != getuserinfo.PassWord)
                        {
                            getuserinfo.PassWord = parameter.PassWord;
                        }
                        if (parameter.NewContactPhone != null && parameter.NewContactPhone != "")
                        {
                            getuserinfo.AccountId = parameter.NewContactPhone;
                            getuserinfo.ContactPhone = parameter.NewContactPhone;
                        }
                        if (parameter.Type != null && parameter.Type != "")
                        {
                            if (getuserinfo.IsManageAdmin != '1')
                            {
                                getuserinfo.Type = parameter.Type;
                            }
                            else
                            {
                                getuserinfo.Type = "1";
                            }
                        }
                        if (parameter.Email != null && parameter.Email != "")
                        {
                            getuserinfo.Email = parameter.Email;
                        }
                        if (parameter.Status != null && parameter.Status != "")
                        {
                            getuserinfo.Status = parameter.Status;
                        }
                        if (parameter.LockState != null && parameter.LockState != "")
                        {
                            getuserinfo.LockState = parameter.LockState;
                        }
                        if (parameter.HeadImgUrl != null && parameter.HeadImgUrl != "")
                        {
                            getuserinfo.HeadImgUrl = parameter.HeadImgUrl;
                        }
                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        r.Msg = "用户信息更新成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 利用ID更新用户信息
        /// </summary>
        public ReturnItem<RetUserInfo> UpdateUserInfo(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (parameter.AccountId != null && parameter.AccountId != "")
                        {
                            getuserinfo.AccountId = parameter.AccountId;
                        }
                        if (parameter.Name != null && parameter.Name != "")
                        {
                            getuserinfo.Name = parameter.Name;
                        }
                        if (parameter.PassWord != null && parameter.PassWord != "" && parameter.PassWord != getuserinfo.PassWord)
                        {
                            getuserinfo.PassWord = parameter.PassWord;
                        }
                        if (parameter.NewContactPhone != null && parameter.NewContactPhone != "")
                        {
                            getuserinfo.AccountId = parameter.NewContactPhone;
                            getuserinfo.ContactPhone = parameter.NewContactPhone;
                        }
                        if (parameter.Type != null && parameter.Type != "")
                        {
                            if (getuserinfo.IsManageAdmin != '1')
                            {
                                getuserinfo.Type = parameter.Type;
                            }
                            else
                            {
                                getuserinfo.Type = "1";
                            }
                        }
                        if (parameter.Email != null && parameter.Email != "")
                        {
                            getuserinfo.Email = parameter.Email;
                        }
                        if (parameter.Status != null && parameter.Status != "")
                        {
                            getuserinfo.Status = parameter.Status;
                        }
                        if (parameter.LockState != null && parameter.LockState != "")
                        {
                            getuserinfo.LockState = parameter.LockState;
                        }
                        if (parameter.HeadImgUrl != null && parameter.HeadImgUrl != "")
                        {
                            getuserinfo.HeadImgUrl = parameter.HeadImgUrl;
                        }
                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        r.Msg = "用户信息更新成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }


        /// <summary>
        /// 获取用户/组织信息
        /// </summary>
        public ReturnItem<RetUserOrganiseInfo> GetInfo(UserOrganizeInfoModel parameter)
        {
            ReturnItem<RetUserOrganiseInfo> r = new ReturnItem<RetUserOrganiseInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        var model = new RetUserOrganiseInfo()
                        {
                            Id = getuserinfo.ID,
                            AccountId = getuserinfo.AccountId.ToString(),
                            Name = getuserinfo.Name.ToString(),
                            Type = getuserinfo.Type,
                            Status = getuserinfo.Status,
                            LockState = getuserinfo.LockState,
                            Email = getuserinfo.Email == null ? "" : getuserinfo.Email.ToString(),
                            ContactPhone = getuserinfo.ContactPhone.ToString(),
                            PassWord = getuserinfo.PassWord.ToString(),
                            IsAdmin = getuserinfo.IsAdmin,
                            IsManageAdmin = getuserinfo.IsManageAdmin,
                            HeadImgUrl = getuserinfo.HeadImgUrl == null ? "" : getuserinfo.HeadImgUrl.ToString(),
                            CreateTime = getuserinfo.CreateTime,
                            UpdateTime = getuserinfo.UpdateTime,
                            LastLoginDate = getuserinfo.LastLoginDate
                        };
                        if (getuserinfo.Type == "2")
                        {
                            using (UserEntities organization = new UserEntities())
                            {
                                try
                                {
                                    var getorganizationinfo = organization.U_Organization.Join(organization.U_UserOrganizationRel, x => x.ID, x => x.OrgID, (a, b) => new { a, b })
                                        .Join(organization.U_User, x => x.b.UserID, x => x.ID, (a, c) => new { a.a, a.b, c })
                                        .Where(x => x.c.AccountId == model.ContactPhone).FirstOrDefault();
                                    if (getorganizationinfo == null)
                                    {
                                        r.Code = -1;
                                        r.Msg = "未找到该组织";
                                        return r;
                                    }
                                    if (getorganizationinfo != null)
                                    {
                                        model.OrganizeId = getorganizationinfo.a.ID.ToString();
                                        model.OrganizeCode = getorganizationinfo.a.Code;
                                        model.OrganizeName = getorganizationinfo.a.Name;
                                        model.OrganizeProvince = getorganizationinfo.a.Province;
                                        model.OrganizeCity = getorganizationinfo.a.City;
                                        model.OrganizeArea = getorganizationinfo.a.Area;
                                        model.OrganizeAddressDetail = getorganizationinfo.a.AddressDetail;
                                        model.OrganizeLogoUrl = getorganizationinfo.a.LogoUrl;
                                        model.OrganizeContactName = getorganizationinfo.a.ContactName;
                                        model.OrganizePhone = getorganizationinfo.a.ContactPhone;
                                        model.OrganizeFixedPhone = getorganizationinfo.a.FixedPhone;
                                        model.OrganizeFax = getorganizationinfo.a.Fax;
                                        model.OrganizeZipCode = getorganizationinfo.a.ZipCode;
                                        model.OrganizeEmail = getorganizationinfo.a.Email;
                                        model.OrganizeState = getorganizationinfo.a.State;
                                        model.OrganizeTradeLevel1 = getorganizationinfo.a.TradeLevel1;
                                        model.OrganizeTradeLevel2 = getorganizationinfo.a.TradeLevel2;
                                        model.OrganizeLocationAddress = getorganizationinfo.a.LocationAddress;
                                        model.OrganizeSite = getorganizationinfo.a.Site;
                                        model.OrganizeDescription = getorganizationinfo.a.Description;
                                        model.OrganizeCreateTime = getorganizationinfo.a.CreateTime;
                                        model.OrganizeUpdateTime = getorganizationinfo.a.UpdateTime;
                                    }
                                }
                                catch (Exception e)
                                {
                                    r.Msg = "内部错误请重试";
                                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                                    r.Code = -1;
                                }
                            }
                        }
                        r.Msg = "用户信息获取成功";
                        r.Code = 0;
                        r.Data = model;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 更新用户/组织信息
        /// </summary>
        public ReturnItem<RetUserOrganiseInfo> UpdateInfo(UserOrganizeInfoModel parameter)
        {
            ReturnItem<RetUserOrganiseInfo> r = new ReturnItem<RetUserOrganiseInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (parameter.AccountId != null && parameter.AccountId != "")
                        {
                            getuserinfo.AccountId = parameter.AccountId;
                        }
                        if (parameter.Name != null && parameter.Name != "")
                        {
                            getuserinfo.Name = parameter.Name;
                        }
                        if (parameter.PassWord != null && parameter.PassWord != "" && parameter.PassWord != getuserinfo.PassWord)
                        {
                            getuserinfo.PassWord = CommonTool.MD5(parameter.PassWord);
                        }
                        if (parameter.ContactPhone != null && parameter.ContactPhone != "")
                        {
                            getuserinfo.ContactPhone = parameter.ContactPhone;
                        }
                        if (parameter.Type != null && parameter.Type != "")
                        {
                            if (getuserinfo.IsManageAdmin != 1)
                            {
                                getuserinfo.Type = parameter.Type;
                            }
                            else
                            {
                                getuserinfo.Type = "1";
                            }
                        }
                        if (parameter.Email != null && parameter.Email != "")
                        {
                            getuserinfo.Email = parameter.Email;
                        }
                        if (parameter.Status != null && parameter.Status != "")
                        {
                            getuserinfo.Status = parameter.Status;
                        }
                        if (parameter.LockState != null && parameter.LockState != "")
                        {
                            getuserinfo.LockState = parameter.LockState;
                        }
                        if (parameter.HeadImgUrl != null && parameter.HeadImgUrl != "")
                        {
                            getuserinfo.HeadImgUrl = parameter.HeadImgUrl;
                        }
                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        if (parameter.Type == "2" && getuserinfo.IsManageAdmin != 1) //类型为“组织”
                        {
                            using (UserEntities organization = new UserEntities())
                            {
                                try
                                {
                                    var getorganizationinfo = organization.U_Organization.Join(organization.U_UserOrganizationRel, x => x.ID, x => x.OrgID, (a, b) => new { a, b })
                                        .Join(organization.U_User, x => x.b.UserID, x => x.ID, (a, c) => new { a.a, a.b, c })
                                        .Where(x => x.c.AccountId == parameter.ContactPhone).FirstOrDefault();
                                    if (getorganizationinfo == null)
                                    {
                                        string num = "";
                                        System.Random Random = new System.Random();
                                        num = Random.Next(0, 99999999).ToString();
                                        U_Organization neworganization = new U_Organization()
                                        {
                                            Code = num,
                                            Name = parameter.OrganizeName == null ? "" : parameter.OrganizeName,
                                            Province = parameter.OrganizeProvince == null ? "" : parameter.OrganizeProvince,
                                            City = parameter.OrganizeCity == null ? "" : parameter.OrganizeCity,
                                            Area = parameter.OrganizeArea == null ? "" : parameter.OrganizeArea,
                                            AddressDetail = parameter.OrganizeAddressDetail == null ? "" : parameter.OrganizeAddressDetail,
                                            LogoUrl = parameter.OrganizeLogoUrl == null ? "" : parameter.OrganizeLogoUrl,
                                            ContactName = parameter.OrganizeContactName == null ? "" : parameter.OrganizeContactName,
                                            ContactPhone = parameter.OrganizePhone == null ? "" : parameter.OrganizePhone,
                                            FixedPhone = parameter.OrganizeFixedPhone == null ? "" : parameter.OrganizeFixedPhone,
                                            Fax = parameter.OrganizeFax == null ? "" : parameter.OrganizeFax,
                                            ZipCode = parameter.OrganizeZipCode == null ? "" : parameter.OrganizeZipCode,
                                            Email = parameter.OrganizeEmail == null ? "" : parameter.OrganizeEmail,
                                            State = "0",
                                            TradeLevel1 = parameter.OrganizeTradeLevel1 == null ? "" : parameter.OrganizeTradeLevel1,
                                            TradeLevel2 = parameter.OrganizeTradeLevel2 == null ? "" : parameter.OrganizeTradeLevel2,
                                            LocationAddress = parameter.OrganizeLocationAddress == null ? "" : parameter.OrganizeLocationAddress,
                                            Site = parameter.OrganizeSite == null ? "" : parameter.OrganizeSite,
                                            Description = parameter.OrganizeDescription == null ? "" : parameter.OrganizeDescription,
                                            CreateTime = DateTime.Now
                                        };
                                        organization.U_Organization.Add(neworganization);
                                        organization.SaveChanges();

                                        var getinfo_1 = organization.U_Organization.OrderByDescending(m => m.CreateTime).FirstOrDefault();
                                        var getinfo_2 = organization.U_User.Where(n => n.AccountId == parameter.ContactPhone).FirstOrDefault();

                                        U_UserOrganizationRel newrel = new U_UserOrganizationRel()
                                        {
                                            UserID = getinfo_2.ID,
                                            OrgID = getinfo_1.ID,
                                            CreateTime = DateTime.Now
                                        };
                                        organization.U_UserOrganizationRel.Add(newrel);
                                        organization.SaveChanges();
                                    }
                                    if (getorganizationinfo != null)
                                    {
                                        getorganizationinfo.a.Name = parameter.OrganizeName == null ? "" : parameter.OrganizeName;
                                        getorganizationinfo.a.Province = parameter.OrganizeProvince == null ? "" : parameter.OrganizeProvince;
                                        getorganizationinfo.a.City = parameter.OrganizeCity == null ? "" : parameter.OrganizeCity;
                                        getorganizationinfo.a.Area = parameter.OrganizeArea == null ? "" : parameter.OrganizeArea;
                                        getorganizationinfo.a.AddressDetail = parameter.OrganizeAddressDetail == null ? "" : parameter.OrganizeAddressDetail;
                                        getorganizationinfo.a.LogoUrl = parameter.OrganizeLogoUrl == null ? "" : parameter.OrganizeLogoUrl;
                                        getorganizationinfo.a.ContactName = parameter.OrganizeContactName == null ? "" : parameter.OrganizeContactName;
                                        getorganizationinfo.a.ContactPhone = parameter.OrganizePhone == null ? "" : parameter.OrganizePhone;
                                        getorganizationinfo.a.FixedPhone = parameter.OrganizeFixedPhone == null ? "" : parameter.OrganizeFixedPhone;
                                        getorganizationinfo.a.Fax = parameter.OrganizeFax == null ? "" : parameter.OrganizeFax;
                                        getorganizationinfo.a.ZipCode = parameter.OrganizeZipCode == null ? "" : parameter.OrganizeZipCode;
                                        getorganizationinfo.a.Email = parameter.OrganizeEmail == null ? "" : parameter.OrganizeEmail;
                                        getorganizationinfo.a.State = "0";
                                        getorganizationinfo.a.TradeLevel1 = parameter.OrganizeTradeLevel1 == null ? "" : parameter.OrganizeTradeLevel1;
                                        getorganizationinfo.a.TradeLevel2 = parameter.OrganizeTradeLevel2 == null ? "" : parameter.OrganizeTradeLevel2;
                                        getorganizationinfo.a.LocationAddress = parameter.OrganizeLocationAddress == null ? "" : parameter.OrganizeLocationAddress;
                                        getorganizationinfo.a.Site = parameter.OrganizeSite == null ? "" : parameter.OrganizeSite;
                                        getorganizationinfo.a.Description = parameter.OrganizeDescription == null ? "" : parameter.OrganizeDescription;
                                        getorganizationinfo.a.UpdateTime = DateTime.Now;
                                        organization.SaveChanges();
                                    }
                                }
                                catch (Exception e)
                                {
                                    r.Msg = "内部错误请重试";
                                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                                    r.Code = -1;
                                }
                            }
                        }
                        r.Msg = "用户信息更新成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 新增用户/组织信息
        /// </summary>
        public ReturnItem<RetUserOrganiseInfo> AddInfo(UserOrganizeInfoModel parameter)
        {
            ReturnItem<RetUserOrganiseInfo> r = new ReturnItem<RetUserOrganiseInfo>();

            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.ContactPhone).FirstOrDefault();
                    if (getuserinfo != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "用户已存在";
                        return r;
                    }
                    if (getuserinfo == null)
                    {
                        U_User newuser = new U_User()
                        {
                            AccountId = parameter.AccountId,
                            Name = parameter.Name,
                            ContactPhone = parameter.ContactPhone,
                            PassWord = CommonTool.MD5(parameter.PassWord),
                            Email = parameter.Email,
                            Type = "2", //组织（必须有组织id）
                            Status = "0",
                            LockState = "0",
                            IsAdmin = 1,
                            IsManageAdmin = parameter.IsManageAdmin,
                            HeadImgUrl = parameter.HeadImgUrl,
                            CreateTime = DateTime.Now
                        };
                        user.U_User.Add(newuser);
                        user.SaveChanges();

                        if (parameter.OrganizeType == "1")
                        {
                            var getinfo_1 = Convert.ToInt32(parameter.OrganizeId);
                            var getinfo_2 = user.U_User.Where(n => n.AccountId == parameter.ContactPhone).FirstOrDefault();

                            U_UserOrganizationRel newrel = new U_UserOrganizationRel()
                            {
                                UserID = getinfo_2.ID,
                                OrgID = getinfo_1,
                                CreateTime = DateTime.Now
                            };
                            user.U_UserOrganizationRel.Add(newrel);
                            user.SaveChanges();
                        }
                        else if (parameter.OrganizeType == "2")
                        {
                            string num = "";
                            System.Random Random = new System.Random();
                            num = Random.Next(0, 99999999).ToString();
                            U_Organization neworganization = new U_Organization()
                            {
                                Code = num,
                                Name = parameter.OrganizeName == null ? "" : parameter.OrganizeName,
                                Province = parameter.OrganizeProvince == null ? "" : parameter.OrganizeProvince,
                                City = parameter.OrganizeCity == null ? "" : parameter.OrganizeCity,
                                Area = parameter.OrganizeArea == null ? "" : parameter.OrganizeArea,
                                AddressDetail = parameter.OrganizeAddressDetail == null ? "" : parameter.OrganizeAddressDetail,
                                LogoUrl = parameter.OrganizeLogoUrl == null ? "" : parameter.OrganizeLogoUrl,
                                ContactName = parameter.OrganizeContactName == null ? "" : parameter.OrganizeContactName,
                                ContactPhone = parameter.OrganizePhone == null ? "" : parameter.OrganizePhone,
                                FixedPhone = parameter.OrganizeFixedPhone == null ? "" : parameter.OrganizeFixedPhone,
                                Fax = parameter.OrganizeFax == null ? "" : parameter.OrganizeFax,
                                ZipCode = parameter.OrganizeZipCode == null ? "" : parameter.OrganizeZipCode,
                                Email = parameter.OrganizeEmail == null ? "" : parameter.OrganizeEmail,
                                State = "0",
                                TradeLevel1 = parameter.OrganizeTradeLevel1 == null ? "" : parameter.OrganizeTradeLevel1,
                                TradeLevel2 = parameter.OrganizeTradeLevel2 == null ? "" : parameter.OrganizeTradeLevel2,
                                LocationAddress = parameter.OrganizeLocationAddress == null ? "" : parameter.OrganizeLocationAddress,
                                Site = parameter.OrganizeSite == null ? "" : parameter.OrganizeSite,
                                Description = parameter.OrganizeDescription == null ? "" : parameter.OrganizeDescription,
                                CreateTime = DateTime.Now
                            };
                            user.U_Organization.Add(neworganization);
                            user.SaveChanges();

                            var getinfo_1 = user.U_Organization.OrderByDescending(m => m.CreateTime).FirstOrDefault();
                            var getinfo_2 = user.U_User.Where(n => n.AccountId == parameter.ContactPhone).FirstOrDefault();

                            U_UserOrganizationRel newrel = new U_UserOrganizationRel()
                            {
                                UserID = getinfo_2.ID,
                                OrgID = getinfo_1.ID,
                                CreateTime = DateTime.Now
                            };
                            user.U_UserOrganizationRel.Add(newrel);
                            user.SaveChanges();
                        }

                        r.Msg = "新增成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 获取用户信息列表数据
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<List<RetUserInfo>> GetUserAllInfo(UserInfoModel parameter)
        {
            ReturnItem<List<RetUserInfo>> r = new ReturnItem<List<RetUserInfo>>();
            List<RetUserInfo> listinfo = new List<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    IQueryable<U_User> userList = user.U_User.AsQueryable<U_User>();
                    if (parameter.Name != null && !"".Equals(parameter.Name))
                    {
                        userList = userList.Where(x => x.Name.IndexOf(parameter.Name) >= 0);
                    }
                    if (!"".Equals(parameter.Type) && parameter.Type != null)
                    {
                        userList = userList.Where(x => x.Type == parameter.Type.ToString());
                    }
                    if (!"".Equals(parameter.Status) && parameter.Status != null)
                    {
                        userList = userList.Where(x => x.Status == parameter.Status.ToString());
                    }
                    if (!"".Equals(parameter.LockState) && parameter.LockState != null)
                    {
                        userList = userList.Where(x => x.LockState == parameter.LockState.ToString());
                    }
                    userList = userList.OrderByDescending(x => x.CreateTime);
                    if (userList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有数据";
                        return r;
                    }
                    if (userList != null)
                    {
                        List<U_User> list = userList.ToList<U_User>();
                        r.Count = userList.Count();
                        r.Msg = "用户信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var userinfo = new RetUserInfo();
                            userinfo.Id = item.ID.ToString();
                            userinfo.AccountId = item.AccountId;
                            userinfo.Name = item.Name;
                            userinfo.ContactPhone = item.ContactPhone.ToString();
                            userinfo.Status = item.Status.ToString();
                            userinfo.LockState = item.LockState.ToString();
                            userinfo.Type = item.Type.ToString() == "1" ? "个人" : "组织";
                            userinfo.CreateTime = item.CreateTime;
                            listinfo.Add(userinfo);
                        }
                        r.Data = listinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                    return r;
                }
            }

            return r;
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        public ReturnItem<RetUserInfo> DeleteUserInfo(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                   
                    // 删除用户表
                    U_User deluser = user.Set<U_User>().Where(a => a.ID == parameter.Id).FirstOrDefault();
                    if (deluser != null)
                    {
                        var entry = user.Entry(deluser);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        user.SaveChanges();
                    }
                    r.Msg = "信息删除成功";
                    r.Code = 0;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 启用/禁用用户
        /// </summary>
        public ReturnItem<RetUserInfo> EnabledUser(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (parameter.Status != null && parameter.Status != "")
                        {
                            getuserinfo.Status = parameter.Status;
                        }

                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        r.Msg = "用户信息更新成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 通过/取消用户
        /// </summary>
        public ReturnItem<RetUserInfo> PassUser(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (parameter.LockState != null && parameter.LockState != "")
                        {
                            getuserinfo.LockState = "0";
                        }

                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        r.Msg = "审核通过";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public ReturnItem<RetUserInfo> ResetPassword(UserInfoModel parameter)
        {
            ReturnItem<RetUserInfo> r = new ReturnItem<RetUserInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.ID == parameter.Id).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        getuserinfo.PassWord = CommonTool.MD5("88888888");
                        getuserinfo.UpdateTime = DateTime.Now;
                        user.SaveChanges();

                        r.Msg = "用户信息更新成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 获取组织信息列表
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<List<RetOrganizationInfo>> GetOrganizationListInfo(OrganizationInfoModel parameter)
        {
            ReturnItem<List<RetOrganizationInfo>> r = new ReturnItem<List<RetOrganizationInfo>>();
            List<RetOrganizationInfo> listinfo = new List<RetOrganizationInfo>();
            using (UserEntities organization = new UserEntities())
            {
                try
                {
                    IQueryable<U_Organization> organizationList = organization.U_Organization.AsQueryable<U_Organization>();
                    if (parameter.Code != null && !"".Equals(parameter.Code))
                    {
                        organizationList = organizationList.Where(x => x.Code.IndexOf(parameter.Code) >= 0);
                    }
                    if (parameter.Name != null && !"".Equals(parameter.Name))
                    {
                        organizationList = organizationList.Where(x => x.Name.IndexOf(parameter.Name) >= 0);
                    }
                    organizationList = organizationList.OrderByDescending(x => x.CreateTime);
                    if (organizationList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到组织信息";
                        return r;
                    }
                    if (organizationList != null)
                    {
                        List<U_Organization> list = organizationList.ToList<U_Organization>();
                        r.Count = organizationList.Count();
                        r.Msg = "组织信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var organizationinfo = new RetOrganizationInfo()
                            {
                                ID = item.ID,
                                Code = item.Code,
                                Name = item.Name,
                                Province = item.Province,
                                City = item.City,
                                Area = item.Area,
                                AddressDetail = item.AddressDetail,
                                LogoUrl = item.LogoUrl,
                                ContactName = item.ContactName,
                                ContactPhone = item.ContactPhone,
                                FixedPhone = item.FixedPhone,
                                Fax = item.Fax,
                                ZipCode = item.ZipCode,
                                Email = item.Email,
                                State = item.State,
                                TradeLevel1 = item.TradeLevel1,
                                TradeLevel2 = item.TradeLevel2,
                                LocationAddress = item.LocationAddress,
                                Site = item.Site,
                                Description = item.Description,
                                CreateTime = item.CreateTime,
                                UpdateTime = item.UpdateTime
                            };
                            // 获取包含的用户
                            var getuserinfo = organization.U_User.Join(organization.U_UserOrganizationRel, x => x.ID, x => x.UserID, (a, b) => new { a, b })
                                .Join(organization.U_Organization, x => x.b.OrgID, x => x.ID, (a, c) => new { a.a, a.b, c })
                                .Where(x => x.c.ID == item.ID).ToList();
                            List<RetUserInfo> userinfo = new List<RetUserInfo>();
                            foreach (var iteminfo in getuserinfo)
                            {
                                RetUserInfo info = new RetUserInfo()
                                {
                                    Id = iteminfo.a.ID.ToString(),
                                    AccountId = iteminfo.a.AccountId.ToString(),
                                    Name = iteminfo.a.Name,
                                    Type = iteminfo.a.Type,
                                    Status = iteminfo.a.Status,
                                    LockState = iteminfo.a.LockState,
                                    Email = iteminfo.a.Email,
                                    ContactPhone = iteminfo.a.ContactPhone,
                                    PassWord = iteminfo.a.PassWord,
                                    IsAdmin = iteminfo.a.IsAdmin,
                                    IsManageAdmin = iteminfo.a.IsManageAdmin,
                                    HeadImgUrl = iteminfo.a.HeadImgUrl == null ? "" : iteminfo.a.HeadImgUrl.ToString(),
                                    CreateTime = iteminfo.a.CreateTime,
                                    UpdateTime = iteminfo.a.UpdateTime,
                                    LastLoginDate = iteminfo.a.LastLoginDate
                                };
                                userinfo.Add(info);
                            }
                            organizationinfo.UserInfoList = userinfo;
                            listinfo.Add(organizationinfo);
                        }
                        r.Data = listinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetOrganizationInfo> GetOrganizationInfo(OrganizationInfoModel parameter)
        {
            ReturnItem<RetOrganizationInfo> r = new ReturnItem<RetOrganizationInfo>();
            using (UserEntities organization = new UserEntities())
            {
                try
                {
                    var getorganizationinfo = organization.U_Organization.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getorganizationinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该组织";
                        return r;
                    }
                    if (getorganizationinfo != null)
                    {
                        r.Msg = "组织信息获取成功";
                        r.Code = 0;
                        r.Data = new RetOrganizationInfo()
                        {
                            ID = getorganizationinfo.ID,
                            Code = getorganizationinfo.Code,
                            Name = getorganizationinfo.Name,
                            Province = getorganizationinfo.Province,
                            City = getorganizationinfo.City,
                            Area = getorganizationinfo.Area,
                            AddressDetail = getorganizationinfo.AddressDetail,
                            LogoUrl = getorganizationinfo.LogoUrl,
                            ContactName = getorganizationinfo.ContactName,
                            ContactPhone = getorganizationinfo.ContactPhone,
                            FixedPhone = getorganizationinfo.FixedPhone,
                            Fax = getorganizationinfo.Fax,
                            ZipCode = getorganizationinfo.ZipCode,
                            Email = getorganizationinfo.Email,
                            State = getorganizationinfo.State,
                            TradeLevel1 = getorganizationinfo.TradeLevel1,
                            TradeLevel2 = getorganizationinfo.TradeLevel2,
                            LocationAddress = getorganizationinfo.LocationAddress,
                            Site = getorganizationinfo.Site,
                            Description = getorganizationinfo.Description,
                            CreateTime = getorganizationinfo.CreateTime,
                            UpdateTime = getorganizationinfo.UpdateTime,
                        };
                        // 获取包含的用户
                        var getuserinfo = organization.U_User.Join(organization.U_UserOrganizationRel, x => x.ID, x => x.UserID, (a, b) => new { a, b })
                            .Join(organization.U_Organization, x => x.b.OrgID, x => x.ID, (a, c) => new { a.a, a.b, c })
                            .Where(x => x.c.ID == parameter.ID).ToList();
                        List<RetUserInfo> userinfo = new List<RetUserInfo>();
                        foreach (var iteminfo in getuserinfo)
                        {
                            RetUserInfo info = new RetUserInfo()
                            {
                                Id = iteminfo.a.ID.ToString(),
                                AccountId = iteminfo.a.AccountId.ToString(),
                                Name = iteminfo.a.Name,
                                Type = iteminfo.a.Type,
                                Status = iteminfo.a.Status,
                                LockState = iteminfo.a.LockState,
                                Email = iteminfo.a.Email,
                                ContactPhone = iteminfo.a.ContactPhone,
                                PassWord = iteminfo.a.PassWord,
                                IsAdmin = iteminfo.a.IsAdmin,
                                IsManageAdmin = iteminfo.a.IsManageAdmin,
                                HeadImgUrl = iteminfo.a.HeadImgUrl == null ? "" : iteminfo.a.HeadImgUrl.ToString(),
                                CreateTime = iteminfo.a.CreateTime,
                                UpdateTime = iteminfo.a.UpdateTime,
                                LastLoginDate = iteminfo.a.LastLoginDate
                            };
                            userinfo.Add(info);
                        }
                        r.Data.UserInfoList = userinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 更新组织信息
        /// </summary>
        public ReturnItem<RetOrganizationInfo> UpdateOrganizationInfo(OrganizationInfoModel parameter)
        {
            ReturnItem<RetOrganizationInfo> r = new ReturnItem<RetOrganizationInfo>();
            using (UserEntities organization = new UserEntities())
            {
                try
                {
                    var getorganizationinfo = organization.U_Organization.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getorganizationinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该组织";
                        return r;
                    }
                    if (getorganizationinfo != null)
                    {
                        getorganizationinfo.Name = parameter.Name == null ? "" : parameter.Name;
                        getorganizationinfo.Province = parameter.Province == null ? "" : parameter.Province;
                        getorganizationinfo.City = parameter.City == null ? "" : parameter.City;
                        getorganizationinfo.Area = parameter.Area == null ? "" : parameter.Area;
                        getorganizationinfo.AddressDetail = parameter.AddressDetail == null ? "" : parameter.AddressDetail;
                        getorganizationinfo.LogoUrl = parameter.LogoUrl == null ? "" : parameter.LogoUrl;
                        getorganizationinfo.ContactName = parameter.ContactName == null ? "" : parameter.ContactName;
                        getorganizationinfo.ContactPhone = parameter.ContactPhone == null ? "" : parameter.ContactPhone;
                        getorganizationinfo.FixedPhone = parameter.FixedPhone == null ? "" : parameter.FixedPhone;
                        getorganizationinfo.Fax = parameter.Fax == null ? "" : parameter.Fax;
                        getorganizationinfo.ZipCode = parameter.ZipCode == null ? "" : parameter.ZipCode;
                        getorganizationinfo.Email = parameter.Email == null ? "" : parameter.Email;
                        getorganizationinfo.State = "0";
                        getorganizationinfo.TradeLevel1 = parameter.TradeLevel1 == null ? "" : parameter.TradeLevel1;
                        getorganizationinfo.TradeLevel2 = parameter.TradeLevel2 == null ? "" : parameter.TradeLevel2;
                        getorganizationinfo.LocationAddress = parameter.LocationAddress == null ? "" : parameter.LocationAddress;
                        getorganizationinfo.Site = parameter.Site == null ? "" : parameter.Site;
                        getorganizationinfo.Description = parameter.Description == null ? "" : parameter.Description;
                        getorganizationinfo.UpdateTime = DateTime.Now;
                        organization.SaveChanges();

                        r.Msg = "组织信息更新成功";
                        r.Code = 0;
                        r.Data = new RetOrganizationInfo()
                        {

                        };
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 新增组织信息
        /// </summary>
        public ReturnItem<RetOrganizationInfo> AddOrganizationInfo(OrganizationInfoModel parameter)
        {
            ReturnItem<RetOrganizationInfo> r = new ReturnItem<RetOrganizationInfo>();
            using (UserEntities organization = new UserEntities())
            {
                try
                {
                    string num = "";
                    System.Random Random = new System.Random();
                    num = Random.Next(0, 99999999).ToString();
                    U_Organization neworganization = new U_Organization()
                    {
                        Code = num,
                        Name = parameter.Name == null ? "" : parameter.Name,
                        Province = parameter.Province == null ? "" : parameter.Province,
                        City = parameter.City == null ? "" : parameter.City,
                        Area = parameter.Area == null ? "" : parameter.Area,
                        AddressDetail = parameter.AddressDetail == null ? "" : parameter.AddressDetail,
                        LogoUrl = parameter.LogoUrl == null ? "" : parameter.LogoUrl,
                        ContactName = parameter.ContactName == null ? "" : parameter.ContactName,
                        ContactPhone = parameter.ContactPhone == null ? "" : parameter.ContactPhone,
                        FixedPhone = parameter.FixedPhone == null ? "" : parameter.FixedPhone,
                        Fax = parameter.Fax == null ? "" : parameter.Fax,
                        ZipCode = parameter.ZipCode == null ? "" : parameter.ZipCode,
                        Email = parameter.Email == null ? "" : parameter.Email,
                        State = "0",
                        TradeLevel1 = parameter.TradeLevel1 == null ? "" : parameter.TradeLevel1,
                        TradeLevel2 = parameter.TradeLevel2 == null ? "" : parameter.TradeLevel2,
                        LocationAddress = parameter.LocationAddress == null ? "" : parameter.LocationAddress,
                        Site = parameter.Site == null ? "" : parameter.Site,
                        Description = parameter.Description == null ? "" : parameter.Description,
                        CreateTime = DateTime.Now
                    };
                    organization.U_Organization.Add(neworganization);
                    organization.SaveChanges();

                    r.Msg = "组织信息新增成功";
                    r.Code = 0;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 删除组织信息
        /// </summary>
        public ReturnItem<RetOrganizationInfo> DeleteOrganizationInfo(OrganizationInfoModel parameter)
        {
            ReturnItem<RetOrganizationInfo> r = new ReturnItem<RetOrganizationInfo>();
            using (UserEntities organization = new UserEntities())
            {
                try
                {
                    // 获取包含的用户
                    var getuserinfo = organization.U_User.Join(organization.U_UserOrganizationRel, x => x.ID, x => x.UserID, (a, b) => new { a, b })
                        .Join(organization.U_Organization, x => x.b.OrgID, x => x.ID, (a, c) => new { a.a, a.b, c })
                        .Where(x => x.c.ID == parameter.ID).ToList();
                    foreach (var item in getuserinfo)
                    {
                        // 删除用户企业关联表
                        U_UserOrganizationRel deluserrel = organization.Set<U_UserOrganizationRel>().Where(a => a.UserID == item.b.ID).FirstOrDefault();
                        if (deluserrel != null)
                        {
                            var entryUserOrg = organization.Entry(deluserrel);
                            //设置该对象的状态为删除  
                            entryUserOrg.State = EntityState.Deleted;
                            organization.SaveChanges();
                        }
                        // 删除用户表
                        U_User deluser = organization.Set<U_User>().Where(a => a.ID == item.a.ID).FirstOrDefault();
                        if (deluser != null)
                        {
                            var entry = organization.Entry(deluser);
                            //设置该对象的状态为删除  
                            entry.State = EntityState.Deleted;
                            organization.SaveChanges();
                        }
                    }
                    // 删除组织表
                    U_Organization delorg = organization.Set<U_Organization>().Where(a => a.ID == parameter.ID).FirstOrDefault();
                    if (delorg != null)
                    {
                        var entry = organization.Entry(delorg);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        organization.SaveChanges();
                    }
                    r.Msg = "信息删除成功";
                    r.Code = 0;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 管理员特定组织下新增用户信息
        /// </summary>
        public ReturnItem<RetUserOrganiseInfo> AddUserInfo(UserOrganizeInfoModel parameter)
        {
            ReturnItem<RetUserOrganiseInfo> r = new ReturnItem<RetUserOrganiseInfo>();

            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(s => s.AccountId == parameter.ContactPhone).FirstOrDefault();
                    if (getuserinfo != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "用户已存在";
                        return r;
                    }
                    if (getuserinfo == null)
                    {
                        U_User newuser = new U_User()
                        {
                            AccountId = parameter.AccountId,
                            Name = parameter.Name,
                            ContactPhone = parameter.ContactPhone,
                            PassWord = CommonTool.MD5(parameter.PassWord),
                            Email = parameter.Email,
                            Type = "2", //组织（必须有组织id）
                            Status = "0",
                            LockState = "0",
                            IsAdmin = 1,
                            IsManageAdmin = parameter.IsManageAdmin,
                            HeadImgUrl = parameter.HeadImgUrl,
                            CreateTime = DateTime.Now
                        };
                        user.U_User.Add(newuser);
                        user.SaveChanges();

                        var getinfo_1 = Convert.ToInt32(parameter.OrganizeId);
                        var getinfo_2 = user.U_User.Where(n => n.AccountId == parameter.ContactPhone).FirstOrDefault();

                        U_UserOrganizationRel newrel = new U_UserOrganizationRel()
                        {
                            UserID = getinfo_2.ID,
                            OrgID = getinfo_1,
                            CreateTime = DateTime.Now
                        };
                        user.U_UserOrganizationRel.Add(newrel);
                        user.SaveChanges();

                        r.Msg = "新增用户成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }
    }
}
