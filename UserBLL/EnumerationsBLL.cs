using Common;
using Common.Config;
using GenerSoft.IndApp.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.Enumerations;
using UserBLL.Model.Return.Enumerations;
using UserDAL;

namespace UserBLL
{
    public class EnumerationsBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 获取下拉数据(一级菜单)
        /// </summary>
        public ReturnItem<List<RetEnumerations>> GetEnumerations(EnumerationsModel parameter)
        {
            ReturnItem<List<RetEnumerations>> r = new ReturnItem<List<RetEnumerations>>();
            if (CustomConfigParam.IsUseRedis)
            {
                RedisUtils redisUtils = new RedisUtils();
                if (redisUtils.isCurMethodCached(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID.ToString(), parameter.GroupName))
                {
                    r.Data = redisUtils.getCacheContent<List<RetEnumerations>>(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID.ToString(), parameter.GroupName);
                    r.Msg = "下拉数据获取成功";
                    r.Code = 0;
                    return r;
                }
            }
            List<RetEnumerations> Enumerations = new List<RetEnumerations>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var list = new List<string>();
                    if (parameter.OrgID != -1)
                    {
                        list.Add("-1");
                        list.Add(parameter.OrgID.ToString());
                    }
                    else
                    {
                        list.Add("-1");
                    }
                    var getinfo = user.U_Enumerations.Where(s => s.GroupName == parameter.GroupName && list.Contains(s.OrgID.ToString())).OrderBy(m => m.Position).ToList();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "下拉数据不存在";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        foreach (var item in getinfo)
                        {
                            RetEnumerations single = new RetEnumerations();
                            single.ID = item.ID;
                            single.Label = item.Label;
                            single.Value = item.Value;
                            single.GroupName = item.GroupName;
                            single.Position = item.Position;
                            single.ParentID = item.ParentID;
                            single.Active = item.Active;
                            single.OrgID = item.OrgID;
                            Enumerations.Add(single);
                        }
                        r.Data = Enumerations;
                        r.Msg = "下拉数据获取成功";
                        r.Code = 0;
                    }
                    if (CustomConfigParam.IsUseRedis)
                    {
                        RedisUtils redisUtil = new RedisUtils();
                        redisUtil.saveToRedis(System.Reflection.MethodBase.GetCurrentMethod(), r.Data, parameter.OrgID.ToString(),parameter.GroupName);
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 获取下拉数据(二级菜单)
        /// </summary>
        public ReturnItem<List<RetEnumerations>> GetSecondLevelEnumerations(EnumerationsModel parameter)
        {
            ReturnItem<List<RetEnumerations>> r = new ReturnItem<List<RetEnumerations>>();
            if (CustomConfigParam.IsUseRedis)
            {
                RedisUtils redisUtils = new RedisUtils();
                if (redisUtils.isCurMethodCached(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID.ToString(), parameter.GroupName))
                {
                    r.Data = redisUtils.getCacheContent<List<RetEnumerations>>(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID.ToString(), parameter.GroupName);
                    r.Msg = "下拉数据获取成功";
                    r.Code = 0;
                    return r;
                }
            }
            List<RetEnumerations> Enumerations = new List<RetEnumerations>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var list = new List<string>();
                    if (parameter.OrgID != -1)
                    {
                        list.Add("-1");
                        list.Add(parameter.OrgID.ToString());
                    }
                    else
                    {
                        list.Add("-1");
                    }
                    var getinfo = user.U_Enumerations.Where(s => s.GroupName == parameter.GroupName && list.Contains(s.OrgID.ToString())).ToList();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "下拉数据不存在";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        foreach (var item in getinfo)
                        {
                            if (item.ParentID == null)
                            {
                                RetEnumerations single = new RetEnumerations();
                                single.ID = item.ID;
                                single.Label = item.Label;
                                single.Value = item.Value;
                                single.GroupName = item.GroupName;
                                single.Position = item.Position;
                                single.ParentID = item.ParentID;
                                single.Active = item.Active;
                                single.OrgID = item.OrgID;
                                Enumerations.Add(single);
                            }
                        }
                        Enumerations = Enumerations.OrderBy(o => o.Position).ToList();//升序
                        foreach (var item in Enumerations)
                        {
                            List<RetEnumerations> secondLevel = new List<RetEnumerations>();
                            foreach (var queue in getinfo)
                            {
                                if (queue.ParentID == item.ID)
                                {

                                    RetEnumerations single = new RetEnumerations();
                                    single.ID = queue.ID;
                                    single.Label = queue.Label;
                                    single.Value = queue.Value;
                                    single.GroupName = queue.GroupName;
                                    single.Position = queue.Position;
                                    single.ParentID = queue.ParentID;
                                    single.Active = queue.Active;
                                    single.OrgID = queue.OrgID;
                                    secondLevel.Add(single);
                                }
                            }
                            item.SecondLevel = secondLevel;
                            item.SecondLevel = item.SecondLevel.OrderBy(o => o.Position).ToList();//升序
                        }
                        r.Data = Enumerations;
                        r.Msg = "下拉数据获取成功";
                        r.Code = 0;
                    }
                    if (CustomConfigParam.IsUseRedis)
                    {
                        RedisUtils redisUtil = new RedisUtils();
                        redisUtil.saveToRedis(System.Reflection.MethodBase.GetCurrentMethod(), r.Data, parameter.OrgID.ToString(), parameter.GroupName);
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }
    }
}
