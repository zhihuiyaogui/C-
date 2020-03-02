using Common;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlarmReport;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlarmReport;
using GenerSoft.IndApp.AlertPoliciesDAL;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    public class AlarmReportInfoBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 获取统计报警信息列表
        /// </summary>
        /// <returns>成功返回列表,失败返回Null.</returns>
        public ReturnItem<List<RetAlarmReport>> GetAlarmReportList(AlarmReportModel parameter, List<RetDeviceInfo> Info)
        {
            ReturnItem<List<RetAlarmReport>> r = new ReturnItem<List<RetAlarmReport>>();
            List<RetAlarmReport> listinfo = new List<RetAlarmReport>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertList = alert.A_AlarmHistory.AsQueryable();
                    if (parameter.OrgID.ToString() != null && parameter.OrgID.ToString() != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        alertList = alertList.Where(s => s.OrgID == OrgID);
                    }
                    var list = alertList.GroupBy(a => a.DeviceID).Select(g => (new { DeviceID = g.Key })).ToList();
                    if (list == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到统计报警信息";
                        return r;
                    }
                    if (list != null)
                    {
                        List<long> idlist = new List<long>();
                        foreach (var id in list)
                        {
                            idlist.Add(id.DeviceID);
                        }
                        var TimeList = alert.A_AlarmHistory.AsQueryable();
                        // 报表周期
                        if (parameter.StartTime != null && parameter.StartTime != "" && parameter.EndTime != null && parameter.EndTime != "")
                        {
                            var StartTime = Convert.ToDateTime(parameter.StartTime);
                            var EndTime = Convert.ToDateTime(parameter.EndTime);
                            TimeList = TimeList.Where(s => s.AlarmTime >= StartTime && s.EndTime <= EndTime);
                        }
                        TimeList = TimeList.Where(s => idlist.Contains(s.DeviceID));
                        foreach (var item in list)
                        {
                            var alertinfo = new RetAlarmReport();
                            alertinfo.DeviceID = item.DeviceID.ToString();
                            // 报警次数、报警总时长
                            Double time = 0;
                            int num = 0;
                            List<string> data = new List<string>();
                            foreach (var info in TimeList)
                            {
                                if (item.DeviceID == info.DeviceID)
                                {
                                    time += (info.EndTime - info.AlarmTime).Value.TotalSeconds;
                                    num++;
                                }
                            }
                            alertinfo.AlarmNumber = num.ToString();
                            data.Add(num.ToString());
                            alertinfo.TotalTime = time.ToString();
                            data.Add(time.ToString());
                            alertinfo.AlertData = data;
                            // 设备名称
                            foreach (var DeviceInfo in Info)
                            {
                                if (DeviceInfo.ID == item.DeviceID)
                                {
                                    if (DeviceInfo.Name.IndexOf(parameter.DeviceName) >= 0)
                                    {
                                        // 设备分组
                                        if (parameter.GroupIDList != null && parameter.GroupIDList.Count > 0)
                                        {
                                            if (parameter.GroupIDList.Count() == 1 && parameter.GroupIDList[0] == null)
                                            {
                                                parameter.GroupIDList[0] = "0";
                                            }
                                            if (parameter.GroupIDList.Contains(DeviceInfo.GroupID))
                                            {
                                                alertinfo.DeviceName = DeviceInfo.Name;
                                                if (num > 0 && time > 0)
                                                {
                                                    // 报警次数
                                                    if (parameter.StartNumber != "" && parameter.EndNumber != "")
                                                    {
                                                        if (num >= Convert.ToInt32(parameter.StartNumber) && num <= Convert.ToInt32(parameter.EndNumber))
                                                        {
                                                            listinfo.Add(alertinfo);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        listinfo.Add(alertinfo);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            alertinfo.DeviceName = DeviceInfo.Name;
                                            if (num > 0 && time > 0)
                                            {
                                                // 报警次数
                                                if (parameter.StartNumber != "" && parameter.EndNumber != "")
                                                {
                                                    if (num >= Convert.ToInt32(parameter.StartNumber) && num <= Convert.ToInt32(parameter.EndNumber))
                                                    {
                                                        listinfo.Add(alertinfo);
                                                    }
                                                }
                                                else
                                                {
                                                    listinfo.Add(alertinfo);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        r.Count = listinfo.Count;
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
        /// 查看单个设备属性报警信息数据列表
        /// </summary>
        /// <returns>成功返回报警记录,失败返回Null.</returns>
        public ReturnItem<RetAlarmReportDetail> GetAlarmReportDetailDataList(AlarmReportItemModel parameter, List<RetDeviceInfo> Info)
        {
            ReturnItem<RetAlarmReportDetail> r = new ReturnItem<RetAlarmReportDetail>();
            RetAlarmReportDetail detail = new RetAlarmReportDetail();
            List<RetAlarmReportItem> listinfo = new List<RetAlarmReportItem>();
            List<RetChartInfo> listchartinfo = new List<RetChartInfo>();
            using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
            {
                try
                {
                    var alertList = alert.A_AlarmHistory.AsQueryable();
                    if (parameter.AlarmTime != null && !"".Equals(parameter.AlarmTime))
                    {
                        alertList = alertList.Where(x => x.AlarmTime >= parameter.AlarmTime);
                    }
                    if (parameter.EndTime != null && !"".Equals(parameter.EndTime))
                    {
                        alertList = alertList.Where(x => x.EndTime <= parameter.EndTime);
                    }
                    if (parameter.OrgID != null && !"".Equals(parameter.OrgID))
                    {
                        var OrgId = Convert.ToInt32(parameter.OrgID);
                        alertList = alertList.Where(x => x.OrgID == OrgId);
                    }
                    if (parameter.DeviceID != null && !"".Equals(parameter.DeviceID))
                    {
                        if (parameter.DeviceItemIDList.Count > 0)
                        {
                            var DeviceID = Convert.ToInt32(parameter.DeviceID);
                            alertList = alertList.Where(s => s.DeviceID == DeviceID && parameter.DeviceItemIDList.Contains(s.DeviceItemID.ToString()));
                        }
                        else
                        {
                            var DeviceID = Convert.ToInt32(parameter.DeviceID);
                            alertList = alertList.Where(s => s.DeviceID == DeviceID);
                        }
                    }
                    alertList = alertList.OrderByDescending(s => s.AlarmTime);
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
                        List<A_AlarmHistory> list = alertList.ToList<A_AlarmHistory>();
                        List<string> compareValue = new List<string>();
                        foreach (var item in list)
                        {
                            var alertinfo = new RetAlarmReportItem();
                            alertinfo.ID = item.ID;
                            alertinfo.DeviceID = item.DeviceID.ToString();
                            alertinfo.DeviceItemID = item.DeviceItemID.ToString();
                            alertinfo.StrategyID = item.StrategyID.ToString();
                            alertinfo.Value = item.Value;
                            alertinfo.AlarmTime = item.AlarmTime;
                            alertinfo.EndTime = item.EndTime;
                            alertinfo.OrgID = item.OrgID.ToString();
                            var chartinfo = new RetChartInfo();
                            foreach (var DeviceInfo in Info)
                            {
                                if (DeviceInfo.ID == item.DeviceID)
                                {
                                    foreach (var DeviceItemInfo in DeviceInfo.DeviceItems)
                                    {
                                        if (DeviceItemInfo.ID == item.DeviceItemID)
                                        {
                                            alertinfo.DeviceName = DeviceInfo.Name;
                                            alertinfo.DeviceItemName = DeviceItemInfo.Name;
                                            listinfo.Add(alertinfo);
                                            if (compareValue == null || (compareValue != null && !compareValue.Exists(x => x == item.DeviceItemID.ToString())))
                                            {
                                                compareValue.Add(item.DeviceItemID.ToString());
                                                chartinfo.DeviceItemName = DeviceItemInfo.Name;
                                                chartinfo.DeviceItemID = DeviceItemInfo.ID;
                                                listchartinfo.Add(chartinfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        detail.AlarmReportItem = listinfo;
                        detail.ChartInfo = listchartinfo;
                        r.Msg = "统计属性报警信息获取成功";
                        r.Code = 0;
                        r.Data = detail;
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
    }
}
