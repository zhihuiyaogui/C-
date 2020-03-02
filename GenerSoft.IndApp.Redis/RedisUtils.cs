using Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GenerSoft.IndApp.Redis
{
    public class RedisUtils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static string RedisExpirSeconds = "864000"; //10天 10*24*60*60

        /// <summary>
        /// 将方法计算所得数据缓存到Redis中
        /// 存入Redis的key格式 包名-类名-方法名-参数-组织ID
        /// </summary>
        /// <param name="method">缓存的方法</param>
        /// <param name="content">缓存的正文</param>
        /// <param name="orgID">组织ID</param>
        /// <param name="parm">方法参数，只接受字符串</param>
        public void saveToRedis(MethodBase method, Object content, string orgID, string parm = "") {
            try
            {
                RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber, RedisExpirSeconds, null);
                string key = getKeyByMethodAndOrgID(method, orgID, parm);
                string value = JsonHelper.ToJson(content);
                redisClient.StringSet(key, content);
                log.InfoFormat("[Redis]Save key: {0}", key);
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误,Save To Redis failed ：{0},{1}", e.Message, e.StackTrace);
            }

        }
        /// <summary>
        /// 当前方法是否被缓存
        /// </summary>
        /// <param name="method"></param>
        /// <param name="orgID"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public bool isCurMethodCached(MethodBase method, string orgID, string parm = "") {
            string key = getKeyByMethodAndOrgID(method, orgID, parm);
            RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber);
            return redisClient.KeyExists(key);
        }

        /// <summary>
        /// 根据方法名和组织获取缓存值
        /// </summary>
        /// <param name="method"></param>
        /// <param name="orgID"></param>
        /// <param name="parm">方法参数，只接受字符串</param>
        /// <returns></returns>
        public T getCacheContent<T>(MethodBase method, string orgID, string parm = "") {
            try
            {
                string key = getKeyByMethodAndOrgID(method, orgID, parm);
                log.InfoFormat("[Redis]Hit key: {0} success!", key);
                RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber);
                return redisClient.StringGet<T>(key);
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误,Get Cache Content failed ：{0},{1}", e.Message, e.StackTrace);
                return default(T);
            }

        }

        /// <summary>
        /// 根据方法名和组织ID生成标准key
        /// </summary>
        /// <param name="method"></param>
        /// <param name="orgID"></param>
        /// <param name="parm">方法参数，只接受字符串</param>
        /// <returns></returns>
        public string getKeyByMethodAndOrgID(MethodBase method, string orgID, string parm) {
            string res = method.DeclaringType.FullName + "." + method.Name + ":OrgID." + orgID;
            if (!string.IsNullOrEmpty(parm))
            {
                res = res + ":" + parm;
            }
            return res;
        }
    }
}
