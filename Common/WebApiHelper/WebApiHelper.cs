using Common.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WebApiHelper
{
    public class WebApiHelper
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 检查请求参数.现在默认只检查Token
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool CheckParameter(WebApiPostParameter parameter)
        {
            bool r = false;
            if (parameter.Content != null)
            {
                r = true;
            }
            return r;
        }
        /// Post请求WebApi获得返回对象
        /// </summary>
        /// <typeparam name="T">返回对象的具体类型</typeparam>
        /// <param name="parameter">请求对象</param>
        /// <param name="debug">出错是否输出错信息到ReturnItem.Message</param>
        /// <param name="mediaType">请求媒体类型</param>
        /// <param name="timeOut">超时时间设置 默认5秒 单位:秒</param>
        /// <returns></returns>
        public ReturnItem<T> GetEntity<T>(WebApiPostParameter parameter, bool debug = false, string mediaType = "application/json", int timeOut = 30)
        {
            if (CheckParameter(parameter))
            {
                string outstr = "";
                int tryintlimit = 5;//尝试5次
                int tryint = 0;//以尝试次数
                bool run = true;
                string responseJson = "";
                #region //规避连接预热产生的超时问题
                while (run)//规避连接预热产生的超时问题
                {
                    try
                    {
                        responseJson = Post(parameter, debug, mediaType, timeOut);
                        try
                        {
                            var get = JsonHelper.JsonToEntity<ReturnItem<T>>(responseJson);
                            run = false;
                            return get;
                        }
                        catch (Exception ex)
                        {
                            //反序列化失败
                            outstr = "错误:" + ex.Message;
                            if (debug)
                            {
                                outstr = outstr + ",接收到的消息:" + responseJson;
                            }
                            log.Debug(outstr);
                            return new ReturnItem<T>() { Code = -1, Msg = outstr };
                        }
                    }
                    catch (Exception ex)
                    {
                        tryint = tryint + 1;
                        if (tryint >= tryintlimit)
                        {
                            run = false;
                            throw new Exception("已达重试次数:" + tryint + "/" + tryintlimit);
                            //return new ReturnItem<T>() { Success = false, Message = "请求失败:超过最大请求次数" };
                        }
                        Thread.Sleep(1000);
                    }
                }
                return null;
                #endregion
            }
            else
            {
                return new ReturnItem<T>() { Code = -1, Msg = "请求失败:WebApePostParameter效验不通过" };
            }
        }
        /// <summary>
        /// 向服务器请求带分页的集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="pageSzie">分页每页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="parameter">请求参数</param>
        /// <param name="debug">测试模式</param>
        /// <param name="mediaType">请求文件返回类型</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public ReturnItem<List<T>> GetList<T>(WebApiPostParameter parameter, bool debug = false, string mediaType = "application/json", int timeOut = 30)
        {
            if (CheckParameter(parameter))
            {
                string outstr = "";
                int tryintlimit = 5;//尝试5次
                int tryint = 0;//以尝试次数
                bool run = true;
                string responseJson = "";
                #region //规避连接预热产生的超时问题
                while (run)//规避连接预热产生的超时问题
                {
                    try
                    {
                        responseJson = Post(parameter, debug, mediaType, timeOut);
                        try
                        {
                            var get = JsonHelper.JsonToEntity<ReturnItem<List<T>>>(responseJson);
                            run = false;
                            return get;
                        }
                        catch (Exception ex)
                        {
                            //反序列化失败
                            outstr = "错误:" + ex.Message;
                            if (debug)
                            {
                                outstr = outstr + ",接收到的消息:" + responseJson;
                            }
                            return new ReturnItem<List<T>>() { Code = -1, Msg = outstr };
                        }
                    }
                    catch (Exception ex)
                    {
                        tryint = tryint + 1;
                        if (tryint >= tryintlimit)
                        {
                            run = false;
                            throw new Exception("已达重试次数:" + tryint + "/" + tryintlimit);
                            //return new ReturnItem<T>() { Success = false, Message = "请求失败:超过最大请求次数" };
                        }
                        Thread.Sleep(1000);
                    }
                }
                return null;
                #endregion
            }
            else
            {
                return new ReturnItem<List<T>>() { Code = -1, Msg = "请求失败:WebApePostParameter效验不通过" };
            }
        }
        internal string Post(WebApiPostParameter parameter, bool debug = false, string mediaType = "application/json", int timeOut = 30)
        {
            string r = "";
            #region 通讯逻辑
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                HttpResponseMessage response = httpClient.PostAsync(parameter.Url, new FormUrlEncodedContent(parameter.Content)).Result;
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        r = response.Content.ReadAsStringAsync().Result;
                        return r;
                    }
                    catch (Exception ex)
                    {
                        return new ReturnItem<object>() { Code = -1, Msg = ex.Message }.ToJson();
                    }
                }
                return new ReturnItem<object>() { Code = -1, Msg = response.StatusCode.ToString() }.ToJson(); ;
            }
            #endregion
        }
        public HttpResponseMessage Connect(WebApiPostParameter parameter, bool debug = false, string mediaType = "application/json", int timeOut = 30)
        {
            #region 通讯逻辑
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                var response = httpClient.SendAsync(new HttpRequestMessage { RequestUri = new Uri(parameter.Url) });
                if (response.Result.IsSuccessStatusCode)
                {
                    try
                    {
                        return response.Result;
                    }
                    catch (Exception ex)
                    {
                        return new System.Net.Http.HttpResponseMessage { };
                    }
                }
                return new HttpResponseMessage { };
            }
            #endregion
        }
    }
}

public class WebApiPostParameter
{
    public WebApiPostParameter()
    {
        Content = new Dictionary<string, string>() { { _tid, TokenKey }, { "ModuleId", CustomConfigParam.ModuleId } };
    }
    public string Url { get; set; }
    /// <summary>
    /// Post的内容主体 
    /// </summary>
    public Dictionary<string, string> Content { get; set; }
    string _tid = "Tid";
    /// <summary>
    /// 请求业务的Token Key 名称 默认Tid: 
    /// </summary>
    public string TokenKey
    {
        get { return CustomConfigParam.WebApiToken; }
    }
    public string ModuleId
    {
        get { return "ModuleId"; }
    }
}