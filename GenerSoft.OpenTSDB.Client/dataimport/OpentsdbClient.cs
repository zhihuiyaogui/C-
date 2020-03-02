using GenerSoft.OpenTSDB.Client.opentsdb.client;
using GenerSoft.OpenTSDB.Client.opentsdb.client.request;
using GenerSoft.OpenTSDB.Client.opentsdb.client.response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{

    /**
     * Opentsdb读写工具类 *
     */
    public class OpentsdbClient
    {
        /**
     * tagv的过滤规则: 精确匹配多项迭代值，多项迭代值以'|'分隔，大小写敏感
     */
        public static string FILTER_TYPE_LITERAL_OR = "literal_or";

        /**
         * tagv的过滤规则: 通配符匹配，大小写敏感
         */
        public static string FILTER_TYPE_WILDCARD = "wildcard";

        /**
         * tagv的过滤规则: 正则表达式匹配
         */
        public static string FILTER_TYPE_REGEXP = "regexp";


        /**
         * tagv的过滤规则: 精确匹配多项迭代值，多项迭代值以'|'分隔，忽略大小写
         */
        public static string FILTER_TYPE_ILITERAL_OR = "iliteral_or";


        /**
         * tagv的过滤规则: 通配符匹配，忽略大小写
         */
        public static string FILTER_TYPE_IWILDCARD = "iwildcard";


        /**
         * tagv的过滤规则: 通配符取非匹配，大小写敏感
         */
        public static string FILTER_TYPE_NOT_LITERAL_OR = "not_literal_or";

        /**
         * tagv的过滤规则: 通配符取非匹配，忽略大小写
         */
        public static string FILTER_TYPE_NOT_ILITERAL_OR = "not_iliteral_or";


        /**
         * tagv的过滤规则:
         * <p/>
         * Skips any time series with the given tag key, regardless of the value.
         * This can be useful for situations where a metric has inconsistent tag sets.
         * NOTE: The filter value must be null or an empty string
         */
        public static string FILTER_TYPE_NOT_KEY = "not_key";


        /**
         * 取平均值的聚合器
         */
        public static string AGGREGATOR_AVG = "avg";
        /**
         * 取累加值的聚合器
         */
        public static string AGGREGATOR_SUM = "sum";
        private HttpClient httpClient;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public OpentsdbClient(string opentsdbUrl)
        {
            this.httpClient = new HttpClientImpl(opentsdbUrl);
        }

        public OpentsdbClient(string opentsdbUrl, string userName, string passWord, string dataConnectID) {

            this.httpClient = new InCloudHttpClientImpl(opentsdbUrl, userName, passWord,dataConnectID);

        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        public bool putData(string metric, DateTime timestamp, long value, Dictionary<string, string> tagMap)
        {
            long timsSecs = (timestamp.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            return this.putData(metric, timsSecs, value, tagMap);
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        public bool putData(string metric, DateTime timestamp, double value, Dictionary<string, string> tagMap)
        {
            long timsSecs = (timestamp.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            return this.putData(metric, timsSecs, value, tagMap);
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        public bool putData(string metric, DateTime timestamp, string value, Dictionary<string, string> tagMap)
        {
            long timsSecs = (timestamp.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            return this.putData(metric, timsSecs, value, tagMap);
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        public bool putData(string metric, DateTime timestamp, object value, Dictionary<string, string> tagMap)
        {
            long timsSecs = (timestamp.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            return this.putData(metric, timsSecs, value, tagMap);
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 转化为秒的时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        private bool putData(string metric, long timestamp, long value, Dictionary<string, string> tagMap)
        {
            MetricBuilder builder = MetricBuilder.getInstance();
            builder.addMetric(metric).setDataPoint(timestamp, value).addTags(tagMap);
            try
            {
                log.Info("write quest：" + builder.build());
                Response response = httpClient.pushMetrics(builder, ExpectResponse.SUMMARY);
                log.Info("response.statusCode:" + response.getStatusCode());
                return response.isSuccess();
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 转化为秒的时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        private bool putData(string metric, long timestamp, double value, Dictionary<string, string> tagMap)
        {
            MetricBuilder builder = MetricBuilder.getInstance();
            builder.addMetric(metric).setDataPoint(timestamp, value).addTags(tagMap);
            try
            {
                log.Info("write quest：" + builder.build());
                Response response = httpClient.pushMetrics(builder, ExpectResponse.SUMMARY);
                log.Info("response.statusCode: " + response.getStatusCode());
                return response.isSuccess();
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /**
         * 写入数据 * * @param metric 指标 * @param timestamp 转化为秒的时间点 * @param value * @param tagMap * @return * @throws Exception
         */
        private bool putData(string metric, long timestamp, string value, Dictionary<string, string> tagMap)
        {
            MetricBuilder builder = MetricBuilder.getInstance();
            builder.addMetric(metric).setDataPoint(timestamp, value).addTags(tagMap);
            try
            {
                log.Info("write quest：" + builder.build());
                Response response = httpClient.pushMetrics(builder, ExpectResponse.SUMMARY);
                log.Info("response.statusCode: " + response.getStatusCode());
                return response.isSuccess();
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /**
        * 写入数据 * * @param metric 指标 * @param timestamp 转化为秒的时间点 * @param value * @param tagMap * @return * @throws Exception
        */
        private bool putData(string metric, long timestamp, object value, Dictionary<string, string> tagMap)
        {
            MetricBuilder builder = MetricBuilder.getInstance();
            builder.addMetric(metric).setDataPoint(timestamp, value).addTags(tagMap);
            try
            {
                log.Info("write quest：" + builder.build());
                Response response = httpClient.pushMetrics(builder, ExpectResponse.SUMMARY);
                log.Info("response.statusCode: " + response.getStatusCode());
                return response.isSuccess();
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /**
         * 查询数据，返回的数据为json格式，结构为： * "[ * " { * " metric: mysql.innodb.row_lock_time, * " tags: { * " host: web01, * " dc: beijing * " }, * " aggregateTags: [], * " dps: { * " 1435716527: 1234, * " 1435716529: 2345 * " } * " }, * " { * " metric: mysql.innodb.row_lock_time, * " tags: { * " host: web02, * " dc: beijing * " }, * " aggregateTags: [], * " dps: { * " 1435716627: 3456 * " } * " } * "]"; * * @param metric 要查询的指标 * @param aggregator 查询的聚合类型, 如: OpentsdbClient.AGGREGATOR_AVG, OpentsdbClient.AGGREGATOR_SUM * @param tagMap 查询的条件 * @param downsample 采样的时间粒度, 如: 1s,2m,1h,1d,2d * @param startTime 查询开始时间,时间格式为yyyy-MM-dd HH:mm:ss * @param endTime 查询结束时间,时间格式为yyyy-MM-dd HH:mm:ss
         */
        public string getData(string metric, Dictionary<string, string> tagMap, string aggregator, string downsample, string startTime, string endTime)
        {
            QueryBuilder queryBuilder = QueryBuilder.getInstance();
            Query query = queryBuilder.getQuery();
            long starttemp = (DateTimeUtil.parse(startTime, "yyyy-MM-dd HH:mm:ss").ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            long endtemp = (DateTimeUtil.parse(endTime, "yyyy-MM-dd HH:mm:ss").ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;

            query.setStart(starttemp);
            query.setEnd(endtemp);
            List<SubQueries> sqList = new List<SubQueries>();
            SubQueries sq = new SubQueries();
            sq.addMetric(metric);
            sq.addTag(tagMap);
            sq.addAggregator(aggregator);
            sq.setDownsample(downsample + "-" + aggregator);
            sqList.Add(sq);
            query.setQueries(sqList);
            try
            {
                log.Info("query request：" + queryBuilder.build()); //这行起到校验作用
                SimpleHttpResponse spHttpResponse = httpClient.pushQueries(queryBuilder, ExpectResponse.DETAIL);
                log.Debug("response.content:" + spHttpResponse.getContent());
                if (spHttpResponse.isSuccess())
                {
                    return spHttpResponse.getContent();
                }
                return null;
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /**
         * 查询数据，返回tags与时序值的映射: Map> * * @param metric 要查询的指标 * @param aggregator 查询的聚合类型, 如: OpentsdbClient.AGGREGATOR_AVG, OpentsdbClient.AGGREGATOR_SUM * @param tagMap 查询的条件 * @param downsample 采样的时间粒度, 如: 1s,2m,1h,1d,2d * @param startTime 查询开始时间, 时间格式为yyyy-MM-dd HH:mm:ss * @param endTime 查询结束时间, 时间格式为yyyy-MM-dd HH:mm:ss * @param retTimeFmt 返回的结果集中，时间点的格式, 如：yyyy-MM-dd HH:mm:ss 或 yyyyMMddHH 等 * @return Map>
         */
        public IDictionary getData(string metric, Dictionary<string, string> tagMap, string aggregator, string downsample, string startTime, string endTime, string retTimeFmt)
        {
            string resContent = this.getData(metric, tagMap, aggregator, downsample, startTime, endTime);
            return this.convertContentToMap(resContent, retTimeFmt);
        }

        public IDictionary convertContentToMap(string resContent, string retTimeFmt)
        {
            IDictionary tagsValuesMap = new Dictionary<string, Dictionary<string, object>>();
            if (resContent == null || "" == resContent.Trim())
            {
                return tagsValuesMap;
            }
            JArray array = JArray.Parse(resContent);
            if (array != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    JObject obj = array[i].ToObject<JObject>();
                    JObject tags = obj["tags"].ToObject<JObject>();
                    JObject dps = obj["dps"].ToObject<JObject>();
                    IDictionary timeValueMap = new Dictionary<string, object>();
                    foreach (var it in dps)
                    {
                        long timestamp = Convert.ToInt64(it.Key.ToString());
                        DateTime datetime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(timestamp); ;
                        timeValueMap.Add(datetime.ToString(retTimeFmt), dps[it.Key]);
                    }
                    tagsValuesMap.Add(tags.ToString(), timeValueMap);
                }
            }
            return tagsValuesMap;
        }

        /// <summary>
        /// 获取某个metric的最新值，默认给出最近24*7小时(一周)的数据，限制回滚时间不超过5000小时，时间过长会导致时序数据库卡死
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="tagMap"></param>
        /// <param name="backScan">获取最新数据回滚时间，单位小时</param>
        /// <returns>参数值</returns>
        public QueryLastResponse queryLastData(string metric, Dictionary<string, string> tagMap, int backScan = 7*24)
        {
            if (backScan > 5000)
            {
                throw new Exception("不支持查询5000h前的最新数据" );
            }
            QueryLast queryLast = new QueryLast();
            queryLast.backScan = backScan;
            queryLast.resolveNames = true;
            SubQueryLast subQueryLst = new SubQueryLast(metric, tagMap);
            queryLast.addSubQuery(subQueryLst);
            try
            {
                SimpleHttpResponse spHttpResponse = httpClient.pushLastQueries(queryLast.build(), ExpectResponse.DETAIL);
                if (spHttpResponse.isSuccess())
                {
                    string res = spHttpResponse.getContent();
                    if (null == res || "" == res)
                    {
                        return null;
                    }
                    else
                    {
                        List<QueryLastResponse> ob = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QueryLastResponse>>(res);
                        if (ob.Count == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return ob[0];
                        }
                    }

                }
                else
                {
                    throw new Exception("get data from opentsdb error: " + spHttpResponse.getContent());
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }
    }
}