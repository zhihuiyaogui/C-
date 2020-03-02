using Common;
using Common.Config;
using GenerSoft.IndApp.CommonSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GenerSoft.IndApp.WebApiFilterAttr
{
    public class EncryptJsonResult : IHttpActionResult
    {
        object content;
        HttpRequestMessage _request;

        public EncryptJsonResult(object value)
        {
            content = value;
        }

        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            try
            {
                if (content != null)
                {
                    if (CustomConfigParam.IsEncrypt)
                    {
                        try
                        {
                            var token = new UserApi().GetTokenEntity();
                            if (token != null)
                            {
                                if (token.Data != null)
                                {
                                    //如果没指定Key Iv则Base64输出
                                    if (!string.IsNullOrWhiteSpace(token.Data.Key) && !string.IsNullOrWhiteSpace(token.Data.IV))
                                    {
                                        var outstr = content.ToJson().ToAesEncryptString(token.Data.Key, token.Data.IV);
                                        //return Ok<string>(outstr);//加密
                                        response = new HttpResponseMessage()
                                        {
                                            Content = new StringContent(outstr, Encoding.GetEncoding("UTF-8"), "text/plain"),
                                            RequestMessage = _request
                                        };
                                    }
                                    else
                                    {
                                        var outstr = content.ToJson().ToBase64EncryptString();
                                        //return Ok<string>(outstr);//加密
                                        response = new HttpResponseMessage()
                                        {
                                            Content = new StringContent(outstr, Encoding.GetEncoding("UTF-8"), "text/plain"),
                                            RequestMessage = _request
                                        };
                                    }
                                }
                                else
                                {
                                    //TokenId被注销后的处理方式
                                    var outstr = content.ToJson().ToBase64EncryptString();
                                    //return Ok<string>(outstr);//加密
                                    response = new HttpResponseMessage()
                                    {
                                        Content = new StringContent(outstr, Encoding.GetEncoding("UTF-8"), "text/plain"),
                                        RequestMessage = _request
                                    };
                                }
                            }
                            else
                            {
                                //return Ok<string>(content.ToJson().ToBase64EncryptString());//加密
                                response = new HttpResponseMessage()
                                {
                                    Content = new StringContent(content.ToJson().ToBase64EncryptString(), Encoding.GetEncoding("UTF-8"), "text/plain"),
                                    RequestMessage = _request
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            //错误友好输出
                            //return Ok<string>(new ReturnItem<object>() { Code = -1, Msg = "会话不合法" }.ToJson());//不加密
                            response = new HttpResponseMessage()
                            {
                                Content = new StringContent(new ReturnItem<object>() { Code = -1, Msg = "会话不合法" }.ToJson(), Encoding.GetEncoding("UTF-8"), "text/plain"),
                                RequestMessage = _request
                            };
                        }
                    }
                    else
                    {
                        //return Ok<string>(content.ToJson());//不加密
                        response = new HttpResponseMessage()
                        {
                            Content = new StringContent(content.ToJson(), Encoding.GetEncoding("UTF-8"), "text/plain"),
                            RequestMessage = _request
                        };
                    }
                }
                else
                {
                    //return Ok<string>(new ReturnItem<object>() { Code = -1, Msg = "未获取到数据!" }.ToJson());
                    response = new HttpResponseMessage()
                    {
                        Content = new StringContent(new ReturnItem<object>() { Code = -1, Msg = "未获取到数据!" }.ToJson(), Encoding.GetEncoding("UTF-8"), "text/plain"),
                        RequestMessage = _request
                    };
                }
            }
            catch (Exception ex)
            {
                //return Ok<string>(new ReturnItem<object>() { Code = -1, Msg = "输出异常" + ex.Message }.ToJson());//不加密
                response = new HttpResponseMessage()
                {
                    Content = new StringContent(new ReturnItem<object>() { Code = -1, Msg = "输出异常" + ex.Message }.ToJson(), Encoding.GetEncoding("UTF-8"), "text/plain"),
                    RequestMessage = _request
                };
            }

            return Task.FromResult(response);
        }
    }
}
