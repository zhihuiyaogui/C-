using Common;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.HistoryAlertPolicies;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.HistoryAlertPolicies;
using GenerSoft.IndApp.AlertPoliciesDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    public class HistoryAlertPoliciesBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 获取历史报警列表记录数据
        /// </summary>
        /// <returns>成功返回报警记录,失败返回Null.</returns>
        public ReturnItem<List<RetHistoryAlertPolicies>> GetHistoryAlertPoliciesDataList(HistoryAlertPoliciesModel parameter, List<RetDeviceInfo> Info)
        {
            ReturnItem<List<RetHistoryAlertPolicies>> r = new ReturnItem<List<RetHistoryAlertPolicies>>();
            List<RetHistoryAlertPolicies> listinfo = new List<RetHistoryAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertList = alert.A_AlarmHistory.Join(alert.A_AlarmStrategy, x => x.StrategyID, x => x.ID, (a, b) => new { a, b }).AsQueryable();
                    if (parameter.StrategyName != null && !"".Equals(parameter.StrategyName))
                    {
                        alertList = alertList.Where(x => x.b.StrategyName.IndexOf(parameter.StrategyName) >= 0);
                    }
                    if (parameter.DeviceID != null && parameter.DeviceID != "")
                    {
                        if (parameter.DeviceItemIDList != null) {
                            if (parameter.DeviceItemIDList.Count > 0)
                            {
                                var DeviceID = Convert.ToInt32(parameter.DeviceID);
                                alertList = alertList.Where(s => s.a.DeviceID == DeviceID && parameter.DeviceItemIDList.Contains(s.a.DeviceItemID.ToString()));
                            }
                        }
                        else
                        {
                            var DeviceID = Convert.ToInt32(parameter.DeviceID);
                            alertList = alertList.Where(s => s.a.DeviceID == DeviceID);
                        }
                    }
                    alertList = alertList.OrderByDescending(s => s.a.AlarmTime);
                    if (parameter.OrgID != null && !"".Equals(parameter.OrgID))
                    {
                        var OrgId = Convert.ToInt32(parameter.OrgID);
                        alertList = alertList.Where(x => x.a.OrgID == OrgId);
                    }
                    if (alertList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有数据";
                        return r;
                    }
                    if (alertList != null)
                    {
                        r.Count = alertList.Count();
                        if (parameter.PageIndex != 0 && parameter.PageSize != 0)
                        {
                            alertList = alertList.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);
                        }
                        var list = alertList.ToList();

                        var compareValue = "";
                        foreach (var item in list)
                        {
                            if("1".Equals(item.b.Compare))
                            {
                                compareValue = "＞";
                            }
                            else if("2".Equals(item.b.Compare))
                            {
                                compareValue = "≥";
                            }
                            else if ("3".Equals(item.b.Compare))
                            {
                                compareValue = "＝";
                            }
                            else if ("4".Equals(item.b.Compare))
                            {
                                compareValue = "＜";
                            }
                            else if ("5".Equals(item.b.Compare))
                            {
                                compareValue = "≤";
                            }
                            else if ("6".Equals(item.b.Compare))
                            {
                                compareValue = "≠";
                            }
                            var alertinfo = new RetHistoryAlertPolicies();
                            alertinfo.ID = item.a.ID;
                            alertinfo.DeviceID = item.a.DeviceID.ToString();
                            alertinfo.DeviceItemID = item.a.DeviceItemID.ToString();
                            alertinfo.StrategyID = item.a.StrategyID.ToString();
                            alertinfo.StrategyName = item.b.StrategyName;
                            alertinfo.Value = item.a.Value;
                            alertinfo.AlarmTime = item.a.AlarmTime;
                            alertinfo.EndTime = item.a.EndTime;
                            alertinfo.OrgID = item.a.OrgID.ToString();
                            alertinfo.Compare = item.b.Compare;
                            if (Info != null) {
                                foreach (var DeviceInfo in Info)
                                {
                                    if (DeviceInfo.ID == item.a.DeviceID)
                                    {
                                        foreach (var DeviceItemInfo in DeviceInfo.DeviceItems)
                                        {
                                            if (DeviceItemInfo.ID == item.a.DeviceItemID)
                                            {
                                                alertinfo.DeviceName = DeviceInfo.Name;
                                                alertinfo.DeviceItemName = DeviceItemInfo.Name;
                                                alertinfo.StrategyValue = DeviceItemInfo.Name + compareValue + item.b.Threshold;
                                                listinfo.Add(alertinfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        r.Msg = "历史报警信息获取成功";
                        r.Code = 0;
                        r.Data = listinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                    return r;
                }
            }
            return r;
        }

        /// <summary>
        /// 通过id删除历史报警列表记录数据
        /// </summary>
        public ReturnItem<RetHistoryAlertPolicies> DelHistoryAlertPoliciesData(HistoryAlertPoliciesModel parameter)
        {
            ReturnItem<RetHistoryAlertPolicies> r = new ReturnItem<RetHistoryAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    A_AlarmHistory delalert = alert.Set<A_AlarmHistory>().Where(a => a.ID == parameter.ID).FirstOrDefault();
                    if (delalert != null)
                    {
                        var entry = alert.Entry(delalert);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        alert.SaveChanges();
                        //保存修改  
                        r.Msg = "信息删除成功";
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
        /// 新增历史报警策略
        /// </summary>
        public ReturnItem<RetHistoryAlertPolicies> AddHistoryAlertPolicies(HistoryAlertPoliciesModel parameter)
        {
            ReturnItem<RetHistoryAlertPolicies> r = new ReturnItem<RetHistoryAlertPolicies>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    //新增历史报警策略
                    A_AlarmHistory newalert = new A_AlarmHistory()
                    {
                        DeviceID = Convert.ToInt32(parameter.DeviceID),
                        DeviceItemID = Convert.ToInt32(parameter.DeviceItemID),
                        StrategyID = Convert.ToInt32(parameter.StrategyID),
                        Value = parameter.Value,
                        AlarmTime = parameter.AlarmTime,
                        EndTime = parameter.EndTime,
                        OrgID = Convert.ToInt32(parameter.OrgID),
                    };
                    alert.A_AlarmHistory.Add(newalert);
                    alert.SaveChanges();

                    r.Msg = "报警策略新增成功";
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
    }
}
