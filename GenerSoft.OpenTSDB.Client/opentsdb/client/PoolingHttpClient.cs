using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class PoolingHttpClient {

       

        public SimpleHttpResponse doPost(string url, string data, string contentType = "application/json")
        {
            SimpleHttpResponse simpleResponse = new SimpleHttpResponse();
            string r = "";
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
            paramList.Add(new KeyValuePair<string, string>("Content", data));
            #region Í¨Ñ¶Âß¼­
            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10000);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                //HttpContent content = new FormUrlEncodedContent(paramList);
                HttpContent content = new StringContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                System.Net.Http.HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                simpleResponse.setStatusCode((int)response.StatusCode);
                try
                {
                    r = response.Content.ReadAsStringAsync().Result;
                    simpleResponse.setContent(r);
                }
                catch (Exception ex)
                {
                    simpleResponse.setContent("´íÎó:" + ex.Message);
                    return simpleResponse;
                }
            }
            return simpleResponse;
            #endregion
        }
    }
}