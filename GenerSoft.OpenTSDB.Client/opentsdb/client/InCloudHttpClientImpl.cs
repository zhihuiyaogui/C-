using Common.Config;
using GenerSoft.IndApp.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client.opentsdb.client
{
    /// <summary>
    /// 适配浪潮云海物联网平台OpenTSDB数据库的客户端
    /// </summary>
    public class InCloudHttpClientImpl :HttpClient
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string serviceUrl = "";

        private string userName;

        private string passWord;

        private string dataConnectID;

        private PoolingHttpClient httpClient = new PoolingHttpClient();

        private static string AUTH_CONTENT_TYPE = "application/x-www-form-urlencoded";

        private static readonly string REDIS_HEADER = "InTSDB:";

        private static int maxTryTimes = 5;

        public InCloudHttpClientImpl(string serviceUrl, string userName, string passWord, string dataConnectID)
        {
            this.serviceUrl = serviceUrl;
            this.userName = userName;
            this.passWord = passWord;
            this.dataConnectID = dataConnectID;
        }



        /// <summary>
        /// 获取token
        /// 调用登录接口获取token
        /// 可以考虑将token缓存
        /// </summary>
        private string GetToken() {

            if (CustomConfigParam.IsUseRedis)
            {
                RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber);
                string token = redisClient.GetString(REDIS_HEADER + this.dataConnectID);
                if (string.IsNullOrEmpty(token))
                {
                    token = this.requestNewToken();
                    redisClient.StringSet(REDIS_HEADER + this.dataConnectID, token);
                }
                else {
                    log.Debug("Redis Hit get Inspur TSDB token,Key:"+REDIS_HEADER + this.dataConnectID);
                }
                return token;

            }
            else {
                return requestNewToken();
            }
        }


        private string requestNewToken() {
            string extendUrl = "/api/auth/token?username=" + this.userName + "&password=" + this.passWord;
            try
            {
                SimpleHttpResponse response = httpClient.doPost(this.serviceUrl + extendUrl, "", AUTH_CONTENT_TYPE);

                if (response.isSuccess())
                {
                    log.Debug("Request new Inspur TSDB token");
                    Dictionary<string, string> token = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.getContent());
                    return token["token"];
                }
                else
                {
                    log.ErrorFormat("获取TSDB token失败,response error:" + response.getContent());
                    throw new Exception("Response error:" + response.getContent());
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("获取TSDB token失败：{0},{1}", e.Message, e.StackTrace);
                throw;
            }
        }

        private string GetAuthUrl(string url) {
            return url + "?token=" + GetToken();
        }

        public override Response pushMetrics(MetricBuilder builder)
        {
            return pushMetrics(builder, ExpectResponse.STATUS_CODE);

        }

        public override Response pushMetrics(MetricBuilder builder, ExpectResponse expectResponse)
        {
            if (builder == null)
            {
                throw new Exception("QueryBuilder 不能为空");
            }
            int tryTime = 1;
            SimpleHttpResponse response = null;
            while (tryTime <= maxTryTimes)
            {
                response = httpClient.doPost(buildUrl(serviceUrl, GetAuthUrl(PUT_POST_API), expectResponse),
                        builder.build());
                if (response.isSuccess())
                {
                    break;
                }
                else {
                    log.ErrorFormat("[InspurTSDB]Get Response Error,Try Times:{0} ,Status Code: {1},Content: {2}", tryTime, response.getStatusCode(), response.getContent());
                    if (response.getStatusCode() == 401)
                    {
                        deleteCurrentToken();
                    }
                    tryTime++;
                }
            }
            

            return getResponse(response);
        }

        public override SimpleHttpResponse pushQueries(QueryBuilder builder)
        {
            return pushQueries(builder, ExpectResponse.STATUS_CODE);

        }

        public override SimpleHttpResponse pushQueries(QueryBuilder builder, ExpectResponse expectResponse)
        {
            if (builder == null)
            {
                throw new Exception("QueryBuilder 不能为空");
            }
            int tryTime = 1;
            SimpleHttpResponse response = null;
            while (tryTime <= maxTryTimes)
            {
                response = httpClient.doPost(buildUrl(serviceUrl, GetAuthUrl(QUERY_POST_API), expectResponse),
                            builder.build());
                if (response.isSuccess())
                {
                    break;
                }
                else
                {
                    log.ErrorFormat("[InspurTSDB]Get Response Error,Try Times:{0} ,Status Code: {1},Content: {2}", tryTime, response.getStatusCode(), response.getContent());
                    if (response.getStatusCode() == 401)
                    {
                        deleteCurrentToken();
                    }
                    tryTime++;
                }
            }
            return response;
        }

        public override SimpleHttpResponse pushLastQueries(QueryBuilder builder)
        {
            return pushLastQueries(builder.build(), ExpectResponse.STATUS_CODE);

        }

        public override SimpleHttpResponse pushLastQueries(String content, ExpectResponse expectResponse)
        {
            if (content == null)
            {
                throw new Exception("content 不能为空");
            }
            int tryTime = 1;
            SimpleHttpResponse response = null;
            while (tryTime <= maxTryTimes)
            {
                response = httpClient.doPost(buildUrl(serviceUrl, GetAuthUrl(QUERY_POST_LAST_API), expectResponse),
                            content);
                if (response.isSuccess())
                {
                    break;
                }
                else
                {
                    log.ErrorFormat("[InspurTSDB]Get Response Error,Try Times:{0} ,Status Code: {1},Content: {2}", tryTime, response.getStatusCode(), response.getContent());
                    if (response.getStatusCode() == 401)
                    {
                        deleteCurrentToken();
                    }
                    tryTime++;
                }
            }
            return response;
        }

        private string buildUrl(string serviceUrl, string postApiEndPoint, ExpectResponse expectResponse)
        {
            string url = serviceUrl + postApiEndPoint;

            switch (expectResponse)
            {
                case ExpectResponse.SUMMARY:
                    url += "&summary";
                    break;
                case ExpectResponse.DETAIL:
                    url += "&details";
                    break;
                default:
                    break;
            }
            return url;
        }

        private Response getResponse(SimpleHttpResponse httpResponse)
        {
            Response response = new Response(httpResponse.getStatusCode());
            string content = httpResponse.getContent();
            if (!string.IsNullOrEmpty(content))
            {
                if (response.isSuccess())
                {
                    ErrorDetail errorDetail = JsonConvert.DeserializeObject<ErrorDetail>(content);
                    response.setErrorDetail(errorDetail);
                }
                else
                {
                    //logger.error("request failed!" + httpResponse);
                    //获取数据失败了，可能是auth 过期，把缓存清掉
                    deleteCurrentToken();
                }
            }
            return response;
        }


        private void deleteCurrentToken() {
            if (CustomConfigParam.IsUseRedis)
            {
                RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber);
                redisClient.KeyDelete(REDIS_HEADER + dataConnectID);
                log.Info("[Redis]delete key:" + REDIS_HEADER + dataConnectID);
            }
        }
    }
    
}
