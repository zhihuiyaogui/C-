using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public abstract class Client {

        public string PUT_POST_API = "/api/put";

        // 查询接口文档地址 http://opentsdb.net/docs/build/html/api_http/query/index.html
        public string QUERY_POST_API = "/api/query";

        // 获取最新数据的API地址
        // 接口文档地址 http://opentsdb.net/docs/build/html/api_http/query/last.html
        public string QUERY_POST_LAST_API = "/api/query/last";


        /**
         * Sends metrics from the builder to the KairosDB server.
         *
         * @param builder
         *            metrics builder
         * @return response from the server
         * @throws IOException
         *             problem occurred sending to the server
         */
        public abstract Response pushMetrics(MetricBuilder builder) ;

        public abstract SimpleHttpResponse pushQueries(QueryBuilder builder) ;


        /// <summary>
        /// 获取最新一条数据
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public abstract SimpleHttpResponse pushLastQueries(QueryBuilder builder);
    }
}