using GenerSoft.IndApp.Redis;
using Common;
using Common.Config;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.Login;
using UserBLL.Model.Return.Login;
using UserBLL.Model.Parameter.User;
using GenerSoft.IndApp.CommonSdk;

namespace User.Controllers
{
    public class LoginController : BaseApiController
    {
        /// <summary>
        /// 1.用户登录
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = false)]
        [HttpPost]
        public IHttpActionResult Login(UserLoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return InspurJson(new ReturnItem<RetUserLoginInfo>() { Code = -1, Msg = "未填写用户名" });
            }
            if (string.IsNullOrWhiteSpace(model.PassWord))
            {
                return InspurJson(new ReturnItem<RetUserLoginInfo>() { Code = -1, Msg = "未填写密码" });
            }
            UserLoginBLL user = new UserLoginBLL();
            var get = user.UserLogin(model);
            UserInfoLoging(get);
            return InspurJson<RetUserLoginInfo>(get);
        }

       

        [HttpPost]
        public IHttpActionResult Logout()
        {
            UserApi api = new UserApi();
            var user = api.GetUserInfoByToken();
            if (user != null)
            {
                if (CustomConfigParam.IsUseRedis)
                {
                    new RedisClient(CustomConfigParam.RedisDbNumber).KeyDelete("Token:" + Guid.Parse(user.Data.TokenId).ToString("N").ToLower());
                }
            }
            return DisableTokenId(new DisableTokenIdParameter() { TokenId = user.Data.TokenId, UserId = user.Data.UserId });

        }
        /// <summary>
        /// 登出操作
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IHttpActionResult DisableTokenId(DisableTokenIdParameter parameter)
        {
            UserLoginBLL user = new UserLoginBLL();
            return InspurJson(user.DisableTokenId(parameter),true);
        }
        object UserInfoLoging(ReturnItem<RetUserLoginInfo> get)
        {
            if (get != null)
            {
                if (get.Code>=0)
                {
                    try
                    {
                        if (CustomConfigParam.IsUseRedis)
                        {
                            ///var redisget = new RedisClient(CustomConfigParam.RedisDbNumber).StringSet<RetUserLoginInfo>("Token:" + get.Data.TokenId.Replace("-", ""), get.Data);
                        }
                        //var getredisdate = redis.StringGet<UserBase>("Token:" + get.Data.UserId);
                    }
                    catch
                    {
                        //redis出错
                        return new ReturnItem<object>() { Code = -1, Msg = "用户信息日志错误" };
                    }
                    return get;
                }
                else
                {
                    return get;
                }
            }
            else
            {
                return new ReturnItem<object>() { Code = -1, Msg = "NoData" };
            }
        }
    }
}
