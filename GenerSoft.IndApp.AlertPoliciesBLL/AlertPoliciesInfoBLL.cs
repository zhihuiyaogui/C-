using Common;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlertPolicies;
using GenerSoft.IndApp.AlertPoliciesDAL;
using Newtonsoft.Json;
using System.Data.Entity;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    public class AlertPoliciesInfoBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 获取报警策略列表
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<List<RetAlertPolicies>> GetAlertPoliciesList(AlertPoliciesModel parameter, List<RetDeviceInfo> Info)
        {
            ReturnItem<List<RetAlertPolicies>> r = new ReturnItem<List<RetAlertPolicies>>();
            List<RetAlertPolicies> listinfo = new List<RetAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertList = alert.A_AlarmStrategy.AsQueryable();
                    if (parameter.StrategyName != null && parameter.StrategyName != "")
                    {
                        alertList = alertList.Where(s => s.StrategyName.IndexOf(parameter.StrategyName) >= 0);
                    }
                    if (parameter.DeviceID != null && parameter.DeviceID != "")
                    {
                        if (parameter.DeviceItemIDList.Count > 0)
                        {
                            var DeviceID = Convert.ToInt32(parameter.DeviceID);
                            alertList = alertList.Where(s => s.DeviceID == DeviceID && parameter.DeviceItemIDList.Contains(s.DeviceItemId.ToString()));
                        }
                        else {
                            var DeviceID = Convert.ToInt32(parameter.DeviceID);
                            alertList = alertList.Where(s => s.DeviceID == DeviceID);
                        }
                    }
                    if (parameter.OrgID.ToString() != null && parameter.OrgID.ToString() != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        alertList = alertList.Where(s => s.OrgID == OrgID);
                    }
                    alertList = alertList.OrderByDescending(s => s.CreateTime);
                    if (alertList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到报警策略";
                        return r;
                    }
                    if (alertList != null)
                    {
                        List<A_AlarmStrategy> list = alertList.ToList<A_AlarmStrategy>();
                        foreach (var item in list)
                        {
                            var alertinfo = new RetAlertPolicies();
                            alertinfo.ID = item.ID.ToString();
                            alertinfo.StrategyName = item.StrategyName;
                            alertinfo.DeviceID = item.DeviceID.ToString();
                            alertinfo.DataConnectID = item.DataConnectID.ToString();
                            // 获取TagList
                            List<RetTagMap> taglist = new List<RetTagMap>();
                            if (item.TagMap != "" && item.TagMap != null)
                            {
                                taglist = JsonConvert.DeserializeObject<List<RetTagMap>>(item.TagMap);
                            }
                            alertinfo.TagList = taglist;

                            alertinfo.DeviceItemId = item.DeviceItemId.ToString();
                            alertinfo.Compare = item.Compare;
                            alertinfo.Threshold = item.Threshold;
                            alertinfo.Remark = item.Remark;
                            alertinfo.CreateTime = item.CreateTime;
                            alertinfo.CreateUserID = item.CreateUserID.ToString();
                            alertinfo.UpdateTime = item.UpdateTime;
                            alertinfo.UpdateUserId = item.UpdateUserId.ToString();
                            alertinfo.Interval = item.Interval.ToString();
                            alertinfo.Active = item.Active.ToString();
                            alertinfo.OrgID = item.OrgID.ToString();
                            foreach (var DeviceInfo in Info)
                            {
                                if (DeviceInfo.ID == item.DeviceID)
                                {
                                    foreach (var DeviceItemInfo in DeviceInfo.DeviceItems)
                                    {
                                        if (DeviceItemInfo.ID == item.DeviceItemId)
                                        {
                                            alertinfo.DeviceName = DeviceInfo.Name;
                                            alertinfo.DeviceItemName = DeviceItemInfo.Name;
                                            listinfo.Add(alertinfo);
                                        }
                                    }
                                }
                            }
                        }

                        r.Count = listinfo.Count();
                        r.Msg = "报警策略信息获取成功";
                        r.Code = 0;
                        r.Data = listinfo;
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
        /// 新增报警策略
        /// </summary>
        public ReturnItem<RetAlertPolicies> AddAlertPoliciesInfo(AlertPoliciesModel parameter)
        {
            ReturnItem<RetAlertPolicies> r = new ReturnItem<RetAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var message = alert.A_AlarmStrategy.Where(s => s.StrategyName == parameter.StrategyName).FirstOrDefault();
                    if (message != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "报警策略已存在";
                        return r;
                    }
                    if (message == null)
                    {
                        string TagMap = "";
                        if (parameter.TagList.Count > 0)
                        {
                            TagMap = JsonConvert.SerializeObject(parameter.TagList);
                        }
                        //新增报警策略
                        A_AlarmStrategy newalert = new A_AlarmStrategy()
                        {
                            StrategyName = parameter.StrategyName,
                            DeviceID = Convert.ToInt32(parameter.DeviceID),
                            DataConnectID = Convert.ToInt32(parameter.DataConnectID),
                            DeviceItemId = Convert.ToInt32(parameter.DeviceItemId),
                            Compare = parameter.Compare,
                            Threshold = parameter.Threshold,
                            Remark = parameter.Remark,
                            CreateUserID = parameter.CreateUserID,
                            CreateTime = DateTime.Now,
                            Interval = Convert.ToInt32(parameter.Interval),
                            OrgID = Convert.ToInt32(parameter.OrgID),
                        };
                        if (TagMap != "")
                        {
                            newalert.TagMap = TagMap;
                        }
                        if (parameter.Active == "1")
                        {
                            newalert.Active = true;
                        }
                        else if (parameter.Active == "0")
                        {
                            newalert.Active = false;
                        }
                        alert.A_AlarmStrategy.Add(newalert);
                        alert.SaveChanges();

                        r.Msg = "报警策略新增成功";
                        r.Code = 0;
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
        /// 新增报警策略列表
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>

        //public ReturnItem<List<RetAlertPolicies>> AddAlertPoliciesInfoList(AlertPoliciesListModel parameter)
        //{
        //    ReturnItem<List<RetAlertPolicies>> r = new ReturnItem<List<RetAlertPolicies>>();
        //    using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
        //    {
        //        try
        //        {
        //            var message = alert.A_AlarmStrategy.Where(s => s.StrategyName == parameter.StrategyName).FirstOrDefault();
        //            if (message != null)
        //            {
        //                r.Data = null;
        //                r.Code = -1;
        //                r.Msg = "报警策略已存在";
        //                return r;
        //            }
        //            if (message == null)
        //            {
        //                foreach (var item in parameter.Property)
        //                {
        //                    string TagMap = "";
        //                    if (item.TagList.Count > 0)
        //                    {
        //                        TagMap = JsonConvert.SerializeObject(item.TagList);
        //                    }
        //                    //新增报警策略
        //                    A_AlarmStrategy newalert = new A_AlarmStrategy()
        //                    {
        //                        StrategyName = parameter.StrategyName,
        //                        DeviceID = Convert.ToInt32(item.DeviceID),
        //                        DataConnectID = Convert.ToInt32(item.DataConnectID),
        //                        DeviceItemId = Convert.ToInt32(item.DeviceItemId),
        //                        Compare = item.Compare,
        //                        Threshold = item.Threshold,
        //                        Remark = parameter.Remark,
        //                        CreateUserID = parameter.CreateUserID,
        //                        CreateTime = DateTime.Now,
        //                        Interval = Convert.ToInt32(parameter.Interval),
        //                        OrgID = Convert.ToInt32(parameter.OrgID),
        //                    };
        //                    if (TagMap != "")
        //                    {
        //                        newalert.TagMap = TagMap;
        //                    }
        //                    if (parameter.Active == "1")
        //                    {
        //                        newalert.Active = true;
        //                    }
        //                    else if (parameter.Active == "0")
        //                    {
        //                        newalert.Active = false;
        //                    }
        //                    alert.A_AlarmStrategy.Add(newalert);
        //                    alert.SaveChanges();

        //                    r.Msg = "报警策略新增成功";
        //                    r.Code = 0;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            r.Msg = "内部错误请重试";
        //            log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
        //            r.Code = -1;
        //        }
        //    }

        //    return r;
        //}

        /// <summary>
        /// 利用ID获取报警策略信息
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetAlertPolicies> GetAlertPoliciesInfoByID(AlertPoliciesModel parameter)
        {
            ReturnItem<RetAlertPolicies> r = new ReturnItem<RetAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertInfo = alert.A_AlarmStrategy.Where(x => x.ID == parameter.ID).FirstOrDefault();
                    if (alertInfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (alertInfo != null)
                    {
                        r.Msg = "设备模板获取成功";
                        r.Code = 0;
                        r.Data = new RetAlertPolicies()
                        {
                            ID = alertInfo.ID.ToString(),
                            StrategyName = alertInfo.StrategyName,
                            DeviceID = alertInfo.DeviceID.ToString(),
                            DataConnectID = alertInfo.DataConnectID.ToString(),
                            DeviceItemId = alertInfo.DeviceItemId.ToString(),
                            Compare = alertInfo.Compare,
                            Threshold = alertInfo.Threshold,
                            Remark = alertInfo.Remark,
                            CreateUserID = alertInfo.CreateUserID.ToString(),
                            CreateTime = alertInfo.CreateTime,
                            UpdateUserId = alertInfo.UpdateUserId.ToString(),
                            UpdateTime = alertInfo.UpdateTime,
                            Interval = alertInfo.Interval.ToString(),
                            OrgID = alertInfo.OrgID.ToString()
                        };
                        if (alertInfo.Active == true)
                        {
                            r.Data.Active = "1";
                        }
                        else if (alertInfo.Active == false)
                        {
                            r.Data.Active = "0";
                        }
                        // 获取TagList
                        List<RetTagMap> taglist = new List<RetTagMap>();
                        if (alertInfo.TagMap != "" && alertInfo.TagMap != null)
                        {
                            taglist = JsonConvert.DeserializeObject<List<RetTagMap>>(alertInfo.TagMap);
                        }
                        r.Data.TagList = taglist;
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
        /// 更新报警策略信息
        /// </summary>
        public ReturnItem<RetAlertPolicies> UpdateAlertPolicies(AlertPoliciesModel parameter)
        {
            ReturnItem<RetAlertPolicies> r = new ReturnItem<RetAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertInfo = alert.A_AlarmStrategy.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (alertInfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备";
                        return r;
                    }
                    if (alertInfo != null)
                    {
                        string TagMap = "";
                        if (parameter.TagList.Count > 0)
                        {
                            TagMap = JsonConvert.SerializeObject(parameter.TagList);
                        }
                        alertInfo.StrategyName = parameter.StrategyName;
                        alertInfo.DeviceID = Convert.ToInt32(parameter.DeviceID);
                        alertInfo.DataConnectID = Convert.ToInt32(parameter.DataConnectID);
                        alertInfo.TagMap = TagMap;
                        alertInfo.DeviceItemId = Convert.ToInt32(parameter.DeviceItemId);
                        alertInfo.Compare = parameter.Compare;
                        alertInfo.Threshold = parameter.Threshold;
                        alertInfo.Remark = parameter.Remark;
                        alertInfo.UpdateTime = DateTime.Now;
                        alertInfo.UpdateUserId = parameter.UpdateUserId;
                        alertInfo.Interval = Convert.ToInt32(parameter.Interval);
                        if (parameter.Active == "1")
                        {
                            alertInfo.Active = true;
                        }
                        else if (parameter.Active == "0")
                        {
                            alertInfo.Active = false;
                        }
                        alertInfo.OrgID = Convert.ToInt32(parameter.OrgID);
                        alert.SaveChanges();

                        r.Msg = "报警策略更新成功";
                        r.Code = 0;
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
        /// 删除报警策略信息
        /// </summary>
        public ReturnItem<RetAlertPolicies> DeleteAlertPolicies(AlertPoliciesModel parameter)
        {
            ReturnItem<RetAlertPolicies> r = new ReturnItem<RetAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var delete = alert.A_AlarmStrategy.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (delete != null)
                    {
                        var entry = alert.Entry(delete);
                        //设置该对象的状态为删除
                        entry.State = EntityState.Deleted;
                        alert.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "报警策略信息删除成功";
                    r.Code = 0;
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
        /// 获取报警策略个数
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<string> GetAlertPoliciesNum(AlertPoliciesModel parameter)
        {
            ReturnItem<string> r = new ReturnItem<string>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var get = alert.A_AlarmStrategy.Where(s => s.OrgID == OrgID).ToList();
                    if (get == null)
                    {
                        r.Msg = "报警策略信息获取成功";
                        r.Code = 0;
                        r.Data = "0";
                        return r;
                    }
                    if (get != null)
                    {
                        r.Msg = "报警策略信息获取成功";
                        r.Code = 0;
                        r.Data = get.Count().ToString();
                        return r;
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
