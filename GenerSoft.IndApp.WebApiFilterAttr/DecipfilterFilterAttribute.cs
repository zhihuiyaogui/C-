using Common;
using Common.Config;
using GenerSoft.IndApp.CommonSdk;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using System.Web.Http.ValueProviders;

namespace GenerSoft.IndApp.WebApiFilterAttr
{
    public class DecipfilterFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 该方法或者控制器是否需要验证登录
        /// </summary>
        public bool NeedLogin { get; set; }
        public bool DisableEncrypt { get; set; }

        private bool _NeedPlatformAdmin = true;
        public bool NeedPlatformAdmin { get { return this._NeedPlatformAdmin; } set { this._NeedPlatformAdmin = value; } }


        public override void OnActionExecuting(HttpActionContext filterContext)
        {

            var ret = CheckSign(filterContext);
            if (ret.Code<0)
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, ret);
                return;
            }

            var token = new UserApi().GetTokenEntity(); //当前用户的Token
            if (NeedLogin)
            {
                if (token == null || token.Data == null)
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ReturnItem<object>() { Code = -1, Msg = "未登陆" });
                    return;
                }

                if (_NeedPlatformAdmin)
                {
                    if (!token.Data.IsPlatformAdmin)
                    {
                        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ReturnItem<object>() { Code = -1, Msg = "非平台管理员，不能执行此操作" });
                        return;
                    }
                }
            }

            if (CustomConfigParam.IsEncrypt)
            {
                #region 解密处理
                dynamic EntityType = null;
                string requestDataStr = ret.Data;
                
                if (token != null && token.Code>=0 && token.Data != null)
                {
                    //制定加密处理方式
                    string dkey = token.Data.Key;
                    string iv = token.Data.IV;
                    if (requestDataStr.Length != 0)
                    {
                        //解密
                        var d = requestDataStr.ToAesDecrypt(dkey, iv);//ToAesDecrypt(get, dkey, iv);
                        //填充对象
                        if (!string.IsNullOrWhiteSpace(d))//检查是否解密成功
                        {
                            //var pds = filterContext.ActionDescriptor.GetParameters().ToList();
                            if (filterContext.ActionArguments.Keys.ToList().Count > 0)
                            {
                                string key = filterContext.ActionArguments.Keys.ToList()[0];
                                EntityType = Activator.CreateInstance(filterContext.ActionDescriptor.GetParameters().First().ParameterType);
                                var get1 = JsonConvert.DeserializeAnonymousType(d, EntityType);
                                filterContext.ActionArguments[key] = get1;
                            }
                        }
                        else
                        {
                            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new ReturnItem<object>() { Code = -1, Msg = "解码出错:" + d });
                            return;
                        }
                    }
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    try
                    {
                        if (token == null && requestDataStr.Length > 0)
                        {

                            //Base64 处理方式
                            var d = requestDataStr.ToBase64DecryptString();
                            //填充对象
                            //var pds = filterContext.ActionDescriptor.GetParameters().ToList();
                            string key = filterContext.ActionArguments.Keys.ToList()[0];
                            EntityType = Activator.CreateInstance(filterContext.ActionDescriptor.GetParameters().First().ParameterType);
                            var get1 = JsonConvert.DeserializeAnonymousType(d, EntityType);
                            filterContext.ActionArguments[key] = get1;

                            base.OnActionExecuting(filterContext);
                        }
                        else
                        {
                            base.OnActionExecuting(filterContext);
                        }
                    }
                    catch(Exception ex)
                    {
                        if (token != null)
                        {
                            //此处暂时以401返回，可调整为其它返回  
                            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new ReturnItem<object>() { Code = -1, Msg = "解码出错:" + requestDataStr });
                            //actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);  
                        }
                        else
                        {
                            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ReturnItem<object>() { Code = -1, Msg = "无效会话" });
                        }
                        return;
                    }
                }
                #endregion
            }
            else
            {
                //正常处理流程
                base.OnActionExecuting(filterContext);
            }
            
        }

        private ReturnItem<string> CheckSign(HttpActionContext filterContext)
        {
            try
            {
                //验证参数
                var temptokenid = filterContext.Request.Headers.GetValues("tokenid");
                var tempsign = filterContext.Request.Headers.GetValues("sign");
                if (temptokenid.Count() <= 0 || tempsign.Count()<=0)
                {
                    return new ReturnItem<string>() { Code = -1, Msg = "签名参数不正确" };
                }
                var getdic = JsonHelper.JsonToEntity<Hashtable>(temptokenid.First().ToBase64DecryptString());

                if (getdic["tokenid"]==null || getdic["randomnum"] ==null || getdic["timestamp"] ==null)
                {
                    return new ReturnItem<string>() { Code = -1, Msg = "签名参数不正确" };
                }
                string tokenId = getdic["tokenid"].ToString();
                string randomNum = getdic["randomnum"].ToString();
                string timeStamp = getdic["timestamp"].ToString();
                string sign = tempsign.First();

                //验证时间戳
                if(!TimeHelper.IsTime(timeStamp,CustomConfigParam.ApiTimeStamp))
                {
                    return new ReturnItem<string>() { Code = -1, Msg = "请求已过期" };
                }

                //获取请求数据  
                Stream stream = filterContext.Request.Content.ReadAsStreamAsync().Result;
                string requestDataStr = "";
                if (stream != null && stream.Length > 0)
                {
                    stream.Position = 0; //当你读取完之后必须把stream的读取位置设为开始
                    using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        requestDataStr = reader.ReadToEnd().ToString();
                    }
                }

                //验证签名
                string datatemp = timeStamp + randomNum + tokenId + requestDataStr;
                if (CommonTool.MD5(datatemp).Replace("-","").ToLower()!=sign)
                {
                    return new ReturnItem<string>() { Code = -1, Msg = "签名不正确" };
                }

                return new ReturnItem<string>() { Code = 0, Msg = "签名验证成功", Data = requestDataStr };
            }
            catch (Exception ex)
            {
                return new ReturnItem<string>() { Code = -1, Msg = "签名认证错误：" + ex.Message, Data = null };
            }
        }
    }
}
