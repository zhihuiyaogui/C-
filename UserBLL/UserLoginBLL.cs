using ChuXin.Redis;
using Common;
using Common.Config;
using GenerSoft.IndApp.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.Login;
using UserBLL.Model.Parameter.User;
using UserBLL.Model.Return.Login;
using UserDAL;

namespace UserBLL
{
    public enum LoginForm
    {
        Web = 1,
        App = 2,
    }
    public class UserLoginBLL
    {
        int pwsmin = 8;
        int pwsmax = 20;

        /// <summary>
        /// 1.登录方法
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pws"></param>
        /// <returns>成功返回用户封装好的对象,失败返回Null.</returns>
        public ReturnItem<RetUserLoginInfo> UserLogin(UserLoginModel parameter)
        {
            ReturnItem<RetUserLoginInfo> r = new ReturnItem<RetUserLoginInfo>();
            if (parameter.PassWord.Length < pwsmin)
            {
                return new ReturnItem<RetUserLoginInfo>() { Code = -1, Msg = "密码小于" + pwsmin + "位" };
            }
            if (parameter.PassWord.Length > pwsmax)
            {
                return new ReturnItem<RetUserLoginInfo>() { Code = -1, Msg = "密码大于" + pwsmax + "位" };
            }
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserinfo = user.U_User.Where(u => u.AccountId == parameter.UserName).FirstOrDefault();
                    if (getuserinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    var datetime = DateTime.Now.AddHours(-1);
                    if (user.U_UserLoginErrorInfo.Where(ule => ule.UserID == getuserinfo.ID && ule.CreateTime > datetime).Count() >= 5)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "密码错误5次,请一小时后再试";
                        return r;
                    }
                    if (getuserinfo != null)
                    {
                        if (getuserinfo.PassWord == parameter.PassWord)
                        {
                            if (getuserinfo.Status != "1")//1 账号禁用 0 账号启用
                            {
                                if (getuserinfo.LockState != "1")//0 已审核 1 未审核
                                {
                                    try
                                    {
                                        getuserinfo.LastLoginDate = DateTime.Now;
                                        //判断是否为移动端,是则发出推送给最后登录的移动端
                                        if (parameter.LoginType == (int)LoginForm.App)
                                        {
                                            try//推送消息(设备更换 踢设备下线)
                                            {
                                            }
                                            catch
                                            {
                                                //推送出错 日志模块未启用.暂时为空
                                            }
                                        }
                                        var newtoken = GetNewToken(getuserinfo.ID, (LoginForm)parameter.LoginType);

                                        //获取组织ID 取默认最新创建的组织
                                        long userorgid = -1;
                                        var Org = user.U_UserOrganizationRel.Where(x => x.UserID == getuserinfo.ID).OrderByDescending(x => x.CreateTime).FirstOrDefault();
                                        if (Org != null)
                                        {
                                            userorgid = Org.OrgID.Value;
                                        }
                                        r.Data = new RetUserLoginInfo()
                                        {
                                            HeadImgUrl = getuserinfo.HeadImgUrl,
                                            UserId = getuserinfo.ID.ToString(),
                                            RealName = getuserinfo.Name,  
                                            Address = "",
                                            BirthDay = "",
                                            CreationDate = getuserinfo.CreateTime.HasValue ? getuserinfo.CreateTime.Value : DateTime.MinValue,
                                            Email = getuserinfo.Email,
                                            ContactPhone = getuserinfo.ContactPhone,
                                            NickName = "",
                                            Phone = "",
                                            AccountId = getuserinfo.AccountId,                                           
                                            Type = getuserinfo.Type,
                                            LocationHref = (getuserinfo.IsManageAdmin == 1) ? "/user/" : "/manage/",
                                            IsPlatformAdmin = getuserinfo.IsManageAdmin == 1 ? true : false,
                                            TokenDisabledTime = newtoken.DisabledTime.HasValue ? newtoken.DisabledTime.Value : DateTime.Now,
                                            OrgID = userorgid
                                        };
                                        r.Code = 0;
                                    }
                                    catch
                                    {
                                        r.Msg = "内部错误请重试";
                                        r.Code = -1;
                                    }
                                }
                                else
                                {
                                    r.Msg = "账号暂未审核，请尝试切换账号登陆";
                                    r.Code = -1;
                                }
                            }
                            else
                            {
                                r.Msg = "账号已被禁用，请尝试切换账号登陆";
                                r.Code = -1;
                            }
                        }
                        else
                        {
                            U_UserLoginErrorInfo uei = new U_UserLoginErrorInfo()
                            {
                                CreateTime = DateTime.Now,
                                UserID = getuserinfo.ID
                            };
                            user.U_UserLoginErrorInfo.Add(uei);
                            user.SaveChanges();
                            //密码错误
                            r.Msg = "密码错误";
                            r.Code = -1;
                        }
                    }
                    else
                    {
                        r.Msg = "找不到用户";
                        r.Code = -1;
                        //找不到用户
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    r.Code = -1;
                }
            }
            r.TimerEnd();
            return r;

        }

        public ReturnItem<RetUserLoginInfo> UserLogout(string tokenid)
        {
            ReturnItem<RetUserLoginInfo> r = new ReturnItem<RetUserLoginInfo>();

            r.TimerEnd();
            return r;
        }

        /// <summary>
        /// 生成新的token(不会保存,需要处理token的业务进行操作)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        U_Token GetNewToken(long userID, LoginForm type)
        {
            //Token失效时间
            int timeout = 2;
            U_Token newtoken =null;
            using (var context = new UserEntities())
            {
                //处理过往token
                var oldtoken = context.U_Token.Where(u => u.UserID == userID).OrderByDescending(a=>a.CreateTime).Take(3).ToList();
                //oldtoken用来推送kick信息
                foreach (var i in oldtoken)
                {
                    //处理redis的session
                    if (CustomConfigParam.IsUseRedis)
                    {
                        try
                        {
                            new RedisClient(CustomConfigParam.RedisDbNumber).KeyDelete("Token:" + i.TokenID.Replace("-", ""));
                        }
                        catch
                        {

                        }
                    }
                    //设置失效
                    i.IsDisabled = true;
                    i.DisabledTime = DateTime.Now;
                }
                //生成新的返回
                newtoken = new U_Token()
                {
                    CreateTime = DateTime.Now,
                    UserID = userID,
                    DisabledTime = DateTime.Now.AddHours(timeout),
                    IsDisabled = false,
                    TokenID = Guid.NewGuid().ToString(),
                    Uniqueness = (type == LoginForm.App ? true : false),
                    UserFrom = (int)type,
                    AesIv = string.Empty.getKey(),
                    AesKey = string.Empty.getKey()
                };
                context.U_Token.Add(newtoken);
                context.SaveChanges();
            }
            return newtoken;
        }

        ///作废TokenId用于注销登录
        public ReturnItem<bool> DisableTokenId(DisableTokenIdParameter parameter)
        {
            if (parameter.TokenId == null || parameter.TokenId == "") { return new ReturnItem<bool>() { Code = -1, Msg = "无TokenId", Data = false }; }
            if (parameter.UserId == null || parameter.UserId == "") { return new ReturnItem<bool>() { Code = -1, Msg = "无user", Data = false }; }
            using (UserEntities user = new UserEntities())
            {
                long temp = Convert.ToInt32(parameter.UserId);
                user.U_Token.Where(t => t.TokenID == parameter.TokenId && !t.IsDisabled == true && t.UserID == temp).ToList().ForEach(t => { t.IsDisabled = true; t.DisabledTime = DateTime.Now; });
                user.SaveChanges();
                if (CustomConfigParam.IsUseRedis)
                {
                    new RedisClient(CustomConfigParam.RedisDbNumber).KeyDelete("Token:" + parameter.TokenId.Replace("-",""));
                }
                return new ReturnItem<bool>() { Code = 0, Msg = "欢迎再来", Data = true };
            }
        }

        public ReturnItem<RetUserLoginInfo> GetUserLoginInfo(GetUserLoginInfoParameter parameter)
        {
            var r = new ReturnItem<RetUserLoginInfo>();
            if (parameter.TokenID == null || parameter.TokenID == "") { r.Code = -1; r.Msg = "TokenId为空"; return r; }
            using (UserEntities user = new UserEntities())
            {
                var token = user.U_Token.Where(t => t.TokenID == parameter.TokenID).FirstOrDefault();
                long temp = Convert.ToInt32(token.UserID);
                var getuserinfo = user.U_User.Where(t => t.ID == temp).FirstOrDefault();

                if (token == null)
                {
                    r.Code = -1;//未找到Token
                    r.Msg = "未找到Token";
                    return r;
                }
                if (token.IsDisabled == true)
                {
                    r.Code = -1;//已关闭的Token
                    r.Msg = "已关闭的Token";
                    return r;
                }
                if (token.DisabledTime <= DateTime.Now)
                {
                    r.Code = -1;//已关失效的Token
                    r.Msg = "已失效的Token";
                    return r;
                }

                r.Code = 0;

                //获取组织ID 取默认最新创建的组织
                long userorgid = -1;
                var Org = user.U_UserOrganizationRel.Where(x => x.UserID == getuserinfo.ID).OrderByDescending(x => x.CreateTime).FirstOrDefault();
                if (Org != null)
                {
                    userorgid = Org.OrgID.Value;
                }

                r.Data = new RetUserLoginInfo()
                {
                    HeadImgUrl = "",
                    
                    AccountId = getuserinfo.AccountId,
                    ContactPhone = getuserinfo.ContactPhone
                };
                return r;
            }
        }
    }
}
