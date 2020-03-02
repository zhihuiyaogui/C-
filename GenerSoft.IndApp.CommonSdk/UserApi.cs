using Common;
using Common.Config;
using Common.WebApiHelper;
using GenerSoft.IndApp.CommonSdk.Model.User;
using GenerSoft.IndApp.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GenerSoft.IndApp.CommonSdk
{
    public class UserApi
    {
        //是否加密传输
        /// <summary>
        /// 获取TokenId详细信息(用于加密)
        /// </summary>
        /// <param name="TokenId"></param>
        /// <returns></returns>
        public ReturnItem<TokenEntity> GetTokenEntity()
        {
            string tokenId = "";
            try
            {
                var getdic = JsonHelper.JsonToEntity<Hashtable>(HttpContext.Current.Request.Headers["tokenid"].ToBase64DecryptString());
                tokenId = getdic["tokenid"].ToString();
                if(tokenId == "")
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            Guid token = Guid.Empty;
            Guid.TryParse(tokenId, out token);

            if (CustomConfigParam.IsUseRedis)
            {
                RetUserLoginInfo getbyRedis = new RedisClient(CustomConfigParam.RedisDbNumber).StringGet<RetUserLoginInfo>("Token:" + tokenId.Replace("-", "").ToLower());
                if (getbyRedis != null)
                {
                    getbyRedis.TokenId = token.ToString("N");

                    //判断是否过期
                    if(getbyRedis.TokenDisabledTime.AddHours(2)<DateTime.Now)
                    {
                        return new ReturnItem<TokenEntity>()
                        {
                            Code = 0,
                            Msg = "登录信息已过期",
                            Data = null
                        };
                    }

                    getbyRedis.TokenDisabledTime = DateTime.Now.AddHours(2);
                    //刷新Redis中的时间
                    new RedisClient(CustomConfigParam.RedisDbNumber).StringSet<RetUserLoginInfo>("Token:" + tokenId.Replace("-", "").ToLower(),getbyRedis);
                    return new ReturnItem<TokenEntity>()
                    {
                        Code = 0,
                        Msg = "获取用户资料成功",
                        Data = new TokenEntity()
                        {
                            IV = getbyRedis.Iv,
                            Key = getbyRedis.Key,
                            UserId = getbyRedis.UserId,
                            TokenId = Guid.Parse(getbyRedis.TokenId),
                            IsPlatformAdmin = getbyRedis.IsPlatformAdmin,
                            DisableDate = getbyRedis.TokenDisabledTime
                        }
                    };
                }
            }

            if (token != Guid.Empty)
            {
                GetTokenEntityParameter parameter = new GetTokenEntityParameter() { TokenId = token };
                WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.UserApiUrl + "Api/User/GetTokenEntity" };
                parameter.SetPostParameter(wparameter);//填充请求参数
                return new WebApiHelper().GetEntity<TokenEntity>(wparameter);
            }
            else
            {
                return null;
                //throw new Exception("不合法的TokenId");
            }
        }

        /// <summary>
        /// 通过TokenId获取用户基本信息
        /// </summary>
        /// <returns></returns>
        public ReturnItem<RetUserLoginInfo> GetUserInfoByToken()
        {
            string tokenId = "";
            var getdic = JsonHelper.JsonToEntity<Hashtable>(HttpContext.Current.Request.Headers["tokenid"].ToBase64DecryptString());
            tokenId = getdic["tokenid"].ToString();

            Guid token = Guid.Empty;
            Guid.TryParse(tokenId, out token);
            if (token != Guid.Empty)
            {
                tokenId = token.ToString("N");
            }
            if (!string.IsNullOrWhiteSpace(tokenId))
            {
                if (CustomConfigParam.IsUseRedis)
                {
                    RetUserLoginInfo getbyRedis = new RedisClient(CustomConfigParam.RedisDbNumber).StringGet<RetUserLoginInfo>("Token:" + tokenId.Replace("-", "").ToLower());
                    if (getbyRedis != null)
                    {
                        return new ReturnItem<RetUserLoginInfo>() { Code = 0, Msg = "获取用户资料成功", Data = getbyRedis };
                    }
                }
                var get = GetUserInfo(new GetUserInfoParameter() { TokenId = token.ToString() });
                if (get.Code >= 0)
                {
                    if (get.Data != null)
                    {
                        if (CustomConfigParam.IsUseRedis)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(tokenId))
                                {
                                    get.Data.TokenId = tokenId;
                                    new RedisClient(CustomConfigParam.RedisDbNumber).StringSet<RetUserLoginInfo>("Token:" + tokenId.Replace("-", "").ToLower(), get.Data);//保存redis
                                }
                            }
                            catch
                            {
                                //redis出错
                            }
                        }
                        return new ReturnItem<RetUserLoginInfo>() { Code = 0, Msg = "获取用户资料成功", Data = get.Data };
                    }
                }
            }
            return new ReturnItem<RetUserLoginInfo>() { Code = -1, Msg = "获取用户资料错误", Data = null };
        }

        /// <summary>
        /// 根据UserId获取用户信息
        /// </summary>
        /// <returns></returns>
        public ReturnItem<RetUserLoginInfo> GetUserInfo(GetUserInfoParameter parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.UserApiUrl + "Api/User/GetUserInfoInside" };
            parameter.SetPostParameter(wparameter);//填充请求参数
            return new WebApiHelper().GetEntity<RetUserLoginInfo>(wparameter);
        }

        /// <summary>
        /// 添加报警信息
        /// </summary>
        /// <returns></returns>
        public ReturnItem<string> AddAlarmMessage(GetMessageInfoParameter parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.UserApiUrl + "Api/Message/AddAlarmMessageInside" };
            wparameter.Content.Add("Type", parameter.Type);
            wparameter.Content.Add("Tittle", parameter.Tittle);
            wparameter.Content.Add("Text", parameter.Text);
            wparameter.Content.Add("OrgID", parameter.OrgID);
            return new WebApiHelper().GetEntity<string>(wparameter);
        }
    }
}
