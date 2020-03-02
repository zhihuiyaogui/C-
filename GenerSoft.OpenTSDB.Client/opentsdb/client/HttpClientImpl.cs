using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class HttpClientImpl : HttpClient
    {

        //private static Logger logger = Logger.getLogger(HttpClientImpl.class);

        private string serviceUrl = "";

        private PoolingHttpClient httpClient = new PoolingHttpClient();

        public HttpClientImpl(string serviceUrl)
        {
            this.serviceUrl = serviceUrl;
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

            SimpleHttpResponse response = httpClient.doPost(buildUrl(serviceUrl, PUT_POST_API, expectResponse),
                        builder.build());

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

            SimpleHttpResponse response = httpClient.doPost(buildUrl(serviceUrl, QUERY_POST_API, expectResponse),
                            builder.build());

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

            SimpleHttpResponse response = httpClient.doPost(buildUrl(serviceUrl, QUERY_POST_LAST_API,expectResponse),
                            content);

            return response;
        }

        private string buildUrl(string serviceUrl, string postApiEndPoint, ExpectResponse expectResponse)
        {
            string url = serviceUrl + postApiEndPoint;

            switch (expectResponse)
            {
                case ExpectResponse.SUMMARY:
                    url += "?summary";
                    break;
                case ExpectResponse.DETAIL:
                    url += "?details";
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
                }
            }
            return response;
        }
    }
}