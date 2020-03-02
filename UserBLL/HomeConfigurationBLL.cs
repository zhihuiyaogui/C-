using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.HomeConfiguration;
using UserBLL.Model.Return.HomeConfiguration;
using UserDAL;

namespace UserBLL
{
    public class HomeConfigurationBLL
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 获取图表列表
        /// </summary>
        /// <returns>成功返回图表列表,失败返回Null.</returns>
        public ReturnItem<List<RetHomeConfiguration>> GetChartList(HomeConfigurationModel parameter)
        {
            ReturnItem<List<RetHomeConfiguration>> r = new ReturnItem<List<RetHomeConfiguration>>();
            List<RetHomeConfiguration> listinfo = new List<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var deviceid = Convert.ToInt32(parameter.DeviceID);
                    var dashboardtype = Convert.ToInt32(parameter.DashBoardType);
                    var getchartlist = new List<U_HomeConfiguration>();
                    if (dashboardtype == 0)
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        getchartlist = user.U_HomeConfiguration.Where(s => s.DashBoardType == dashboardtype && s.OrgID == OrgID).ToList();
                    }
                    else if (dashboardtype == 1)
                    {
                        getchartlist = user.U_HomeConfiguration.Where(s => s.DeviceID == deviceid && s.DashBoardType == dashboardtype).ToList();
                    }
                    if (getchartlist.Count == 0 || getchartlist == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备图表";
                        return r;
                    }
                    if (getchartlist != null && getchartlist.Count != 0)
                    {
                        List<U_HomeConfiguration> list = getchartlist.ToList<U_HomeConfiguration>();
                        r.Count = getchartlist.Count();
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var chartlistinfo = new RetHomeConfiguration();
                            chartlistinfo.ID = item.ID;
                            chartlistinfo.DashBoardType = item.DashBoardType.ToString();
                            chartlistinfo.DeviceID = item.DeviceID.ToString();
                            chartlistinfo.DeviceItemID = item.DeviceItemID.ToString();
                            chartlistinfo.ChartName = item.ChartName.ToString();
                            chartlistinfo.ChartType = item.ChartType.ToString();
                            // ChartConfig赋值
                            chartlistinfo.ChartConfig = item.ChartConfig;
                            DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel();
                            if (item.ChartConfig != "" && item.ChartConfig != null)
                            {
                                ChartConfig = JsonConvert.DeserializeObject<DashBoardChartConfigModel>(item.ChartConfig);
                            }
                            chartlistinfo.DatabaseType = ChartConfig.DatabaseType;
                            chartlistinfo.DataType = ChartConfig.DataType;
                            chartlistinfo.RecentInterval = ChartConfig.RecentInterval;
                            chartlistinfo.RecentUnit = ChartConfig.RecentUnit;
                            if (ChartConfig.StartTime != "")
                            {
                                chartlistinfo.StartTime = Convert.ToDateTime(ChartConfig.StartTime);
                            }
                            if(ChartConfig.EndTime != "")
                            {
                                chartlistinfo.EndTime = Convert.ToDateTime(ChartConfig.EndTime);
                            }
                            chartlistinfo.StatisticalInterval = ChartConfig.StatisticalInterval;
                            chartlistinfo.IntervalUnit = ChartConfig.IntervalUnit;
                            chartlistinfo.MinValue = ChartConfig.MinValue;
                            chartlistinfo.MaxValue = ChartConfig.MaxValue;
                            chartlistinfo.ValueType = ChartConfig.ValueType;
                            chartlistinfo.TextColor = ChartConfig.TextColor;
                            chartlistinfo.BackgroundColor = ChartConfig.BackgroundColor;
                            chartlistinfo.HomeDeviceInfoList = ChartConfig.HomeDeviceInfoList;
                            //首页
                            if (item.DashBoardType == 0)
                            {
                                List<BaseModel> Property = new List<BaseModel>();
                                if (ChartConfig.SelectionType == "1") //设备
                                {
                                    foreach (var info in ChartConfig.HomeDeviceInfoList)
                                    {
                                        BaseModel model = new BaseModel();
                                        List<string> data = new List<string>();
                                        data.Add("1");
                                        data.Add(info.DeviceID);
                                        data.Add(info.DeviceItemID);
                                        model.data = data;
                                        Property.Add(model);
                                    }
                                }
                                else if (ChartConfig.SelectionType == "2")//总览
                                {
                                    BaseModel model = new BaseModel();
                                    List<string> data = new List<string>();
                                    data.Add("2");
                                    data.Add(ChartConfig.ValueType);
                                    model.data = data;
                                    Property.Add(model);
                                }
                                else if (ChartConfig.SelectionType == "3")//表格
                                {
                                    BaseModel model = new BaseModel();
                                    List<string> data = new List<string>();
                                    data.Add("3");
                                    data.Add(ChartConfig.ValueType);
                                    model.data = data;
                                    Property.Add(model);
                                }
                                chartlistinfo.Property = Property;
                            }
                            //设备
                            else if (item.DashBoardType == 1)
                            {
                                List<DeviceBaseModel> DeviceItemList = new List<DeviceBaseModel>();
                                foreach (var info in ChartConfig.HomeDeviceInfoList)
                                {
                                    DeviceBaseModel model = new DeviceBaseModel();
                                    model.DeviceItemID = info.DeviceItemID;
                                    DeviceItemList.Add(model);
                                }
                                chartlistinfo.DeviceItemList = DeviceItemList;
                            }
                            // DashBoardData赋值
                            chartlistinfo.DashBoardData = item.DashBoardData;
                            RetDashBoardDataList DashBoardData = new RetDashBoardDataList();
                            if (item.DashBoardData != "" && item.DashBoardData != null)
                            {
                                DashBoardData = JsonConvert.DeserializeObject<RetDashBoardDataList>(item.DashBoardData);
                            }
                            chartlistinfo.DashBoardDataList = DashBoardData;
                            chartlistinfo.Remark = item.Remark;
                            chartlistinfo.SortID = item.SortID;
                            chartlistinfo.CreateTime = item.CreateTime;
                            chartlistinfo.CreateUserID = item.CreateUserID.ToString();
                            chartlistinfo.OrgID = item.OrgID.ToString();
                            listinfo.Add(chartlistinfo);
                        }
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
        /// 利用ID获取图表列表
        /// </summary>
        public ReturnItem<RetHomeConfiguration> GetChart(HomeConfigurationModel parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getchart = user.U_HomeConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getchart == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备图表";
                        return r;
                    }
                    if (getchart != null)
                    {
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        r.Data = new RetHomeConfiguration()
                        {
                            ID = getchart.ID,
                            DashBoardType = getchart.DashBoardType.ToString(),
                            DeviceID = getchart.DeviceID.ToString(),
                            DeviceItemID = getchart.DeviceItemID.ToString(),
                            ChartName = getchart.ChartName.ToString(),
                            ChartType = getchart.ChartType.ToString(),
                            ChartConfig = getchart.ChartConfig,
                            DashBoardData = getchart.DashBoardData,
                            Remark = getchart.Remark,
                            SortID = getchart.SortID,
                            CreateTime = getchart.CreateTime,
                            CreateUserID = getchart.CreateUserID.ToString(),
                            OrgID = getchart.OrgID.ToString()
                        };
                        // DashBoardData赋值
                        RetDashBoardDataList DashBoardData = new RetDashBoardDataList();
                        if (getchart.DashBoardData != "" && getchart.DashBoardData != null)
                        {
                            DashBoardData = JsonConvert.DeserializeObject<RetDashBoardDataList>(getchart.DashBoardData);
                        }
                        r.Data.DashBoardDataList = DashBoardData;
                        r.Data.XAxis = DashBoardData.x.ToString();
                        r.Data.YAxis = DashBoardData.y.ToString();
                        r.Data.Width = DashBoardData.w.ToString();
                        r.Data.Height = DashBoardData.h.ToString();
                        // ChartConfig赋值
                        DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel();
                        if (getchart.ChartConfig != "" && getchart.ChartConfig != null)
                        {
                            ChartConfig = JsonConvert.DeserializeObject<DashBoardChartConfigModel>(getchart.ChartConfig);
                        }
                        r.Data.DatabaseType = ChartConfig.DatabaseType;
                        r.Data.DataType = ChartConfig.DataType;
                        r.Data.RecentInterval = ChartConfig.RecentInterval;
                        r.Data.RecentUnit = ChartConfig.RecentUnit;
                        if (ChartConfig.StartTime != "")
                        {
                            r.Data.StartTime = Convert.ToDateTime(ChartConfig.StartTime);
                        }
                        if (ChartConfig.EndTime != "")
                        {
                            r.Data.EndTime = Convert.ToDateTime(ChartConfig.EndTime);
                        }
                        r.Data.StatisticalInterval = ChartConfig.StatisticalInterval;
                        r.Data.IntervalUnit = ChartConfig.IntervalUnit;
                        r.Data.MinValue = ChartConfig.MinValue;
                        r.Data.MaxValue = ChartConfig.MaxValue;
                        r.Data.ValueType = ChartConfig.ValueType;
                        r.Data.TextColor = ChartConfig.TextColor;
                        r.Data.BackgroundColor = ChartConfig.BackgroundColor;
                        r.Data.HomeDeviceInfoList = ChartConfig.HomeDeviceInfoList;
                        //首页
                        if (getchart.DashBoardType == 0)
                        {
                            List<BaseModel> Property = new List<BaseModel>();
                            if (ChartConfig.SelectionType == "1") //设备
                            {
                                foreach (var info in ChartConfig.HomeDeviceInfoList)
                                {
                                    BaseModel model = new BaseModel();
                                    List<string> data = new List<string>();
                                    data.Add("1");
                                    data.Add(info.DeviceID);
                                    data.Add(info.DeviceItemID);
                                    model.data = data;
                                    Property.Add(model);
                                }
                            }
                            else if (ChartConfig.SelectionType == "2")//总览
                            {
                                BaseModel model = new BaseModel();
                                List<string> data = new List<string>();
                                data.Add("2");
                                data.Add(ChartConfig.ValueType);
                                model.data = data;
                                Property.Add(model);
                            }
                            else if (ChartConfig.SelectionType == "3")//表格
                            {
                                BaseModel model = new BaseModel();
                                List<string> data = new List<string>();
                                data.Add("3");
                                data.Add(ChartConfig.ValueType);
                                model.data = data;
                                Property.Add(model);
                            }
                            r.Data.Property = Property;
                        }
                        //设备
                        else if (getchart.DashBoardType == 1)
                        {
                            List<DeviceBaseModel> DeviceItemList = new List<DeviceBaseModel>();
                            foreach (var info in ChartConfig.HomeDeviceInfoList)
                            {
                                DeviceBaseModel model = new DeviceBaseModel();
                                model.DeviceItemID = info.DeviceItemID;
                                DeviceItemList.Add(model);
                            }
                            r.Data.DeviceItemList = DeviceItemList;
                        }
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
        /// 验证图表是否存在（删除设备属性）
        /// </summary>
        public ReturnItem<RetHomeConfiguration> CheckChart(List<HomeConfigurationModel> parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    foreach (var item in parameter)
                    {
                        List<int?> DashBoardType = new List<int?>() { 0, 1, 4 };
                        var DeviceID = Convert.ToInt32(item.DeviceID);
                        var DeviceItemID = Convert.ToInt32(item.DeviceItemID);
                        var ss = "\"DeviceID\":\"" + DeviceID + "\",\"DeviceItemID\":\""+ DeviceItemID + "\"";
                        var getchart = user.U_HomeConfiguration.Where(s => DashBoardType.Contains(s.DashBoardType) && s.ChartConfig.Contains(ss)).FirstOrDefault();
                        if (getchart == null)
                        {
                            r.Data = null;
                            r.Code = 0;
                            r.Msg = "未找到设备图表";
                        }
                        else if (getchart != null)
                        {
                            r.Data = null;
                            r.Code = 1;
                            r.Msg = "设备图表已存在";
                            break;
                        }
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
        /// 新增图表
        /// </summary>
        public ReturnItem<RetHomeConfiguration> AddChart(HomeConfigurationModel parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var DashBoardType = Convert.ToInt32(parameter.DashBoardType);
                    var addchart = user.U_HomeConfiguration.Where(s => s.ChartName == parameter.ChartName && s.OrgID == OrgID && s.DashBoardType == DashBoardType).FirstOrDefault();
                    if (addchart != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备图表已存在";
                        return r;
                    }
                    if (addchart == null)
                    {
                        U_HomeConfiguration newChart = new U_HomeConfiguration()
                        {
                            DashBoardType = Convert.ToInt32(parameter.DashBoardType),
                            ChartName = parameter.ChartName,
                            ChartType = Convert.ToInt32(parameter.ChartType),
                            ChartConfig = parameter.ChartConfig,
                            DashBoardData = parameter.DashBoardData,
                            Remark = parameter.Remark,
                            SortID = Convert.ToInt32(parameter.SortID),
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID)
                        };
                        if (parameter.DeviceID != "" && parameter.DeviceID != null)
                        {
                            newChart.DeviceID = Convert.ToInt32(parameter.DeviceID);
                        }
                        user.U_HomeConfiguration.Add(newChart);
                        user.SaveChanges();

                        var getchart = user.U_HomeConfiguration.Where(s => s.ChartName == parameter.ChartName && s.OrgID == OrgID).FirstOrDefault();
                        if (getchart != null)
                        {
                            DashBoardDataListModel DashBoardData = new DashBoardDataListModel();
                            if (getchart.DashBoardData != "" && getchart.DashBoardData != null)
                            {
                                DashBoardData = JsonConvert.DeserializeObject<DashBoardDataListModel>(getchart.DashBoardData);
                            }
                            DashBoardData.i = getchart.ID.ToString();
                            getchart.DashBoardData = JsonConvert.SerializeObject(DashBoardData);
                        }
                        user.SaveChanges();
                        r.Msg = "设备信息新增成功";
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
        /// 更新图表
        /// </summary>
        public ReturnItem<RetHomeConfiguration> EditChart(HomeConfigurationModel parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var updatechart = user.U_HomeConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (updatechart == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备图表不存在";
                        return r;
                    }
                    if (updatechart != null)
                    {
                        if (parameter.DeviceID != "" && parameter.DeviceID != null)
                        {
                            updatechart.DeviceID = Convert.ToInt32(parameter.DeviceID);
                        }
                        updatechart.DashBoardType = Convert.ToInt32(parameter.DashBoardType);
                        updatechart.ChartName = parameter.ChartName;
                        updatechart.ChartType = Convert.ToInt32(parameter.ChartType);
                        updatechart.ChartConfig = parameter.ChartConfig;
                        updatechart.DashBoardData = parameter.DashBoardData;
                        updatechart.Remark = parameter.Remark;
                        user.SaveChanges();
                        r.Msg = "图表信息更新成功";
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
        /// 删除图表
        /// </summary>
        public ReturnItem<RetHomeConfiguration> DeleteChart(HomeConfigurationModel parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var deletechart = user.U_HomeConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (deletechart != null)
                    {
                        var entry = user.Entry(deletechart);
                        //设置该对象的状态为删除
                        entry.State = EntityState.Deleted;
                        user.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "图表删除成功";
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
        /// 保存布局
        /// </summary>
        public ReturnItem<RetHomeConfiguration> SaveLayout(List<DashBoardDataListModel> list)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    foreach (var item in list)
                    {
                        var id = Convert.ToInt32(item.i);
                        var savechart = user.U_HomeConfiguration.Where(s => s.ID == id).FirstOrDefault();
                        if (savechart != null)
                        {
                            DashBoardDataListModel DashBoardData = new DashBoardDataListModel()
                            {
                                x = item.x,
                                y = item.y,
                                w = item.w,
                                h = item.h,
                                i = item.i
                            };
                            savechart.DashBoardData = JsonConvert.SerializeObject(DashBoardData);
                            user.SaveChanges();
                        }
                    }
                    r.Msg = "布局保存成功";
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
        /// 查询设备图表名称是否存在
        /// </summary>
        public ReturnItem<RetHomeConfiguration> CheckDeviceInfoByChartName(HomeConfigurationModel parameter)
        {
            ReturnItem<RetHomeConfiguration> r = new ReturnItem<RetHomeConfiguration>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var checkchart = user.U_HomeConfiguration.Where(s => s.ChartName == parameter.ChartName && s.OrgID == OrgID).FirstOrDefault();
                    if (checkchart == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到设备图表";
                        return r;
                    }
                    if (checkchart != null)
                    {
                        r.Msg = "已存在设备图表";
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
        /// 实时监控页面新增数据位置
        /// </summary>
        public ReturnItem<RetRealTimeMonitor> AddRealTimeMonitor(RealTimeMonitorModel parameter)
        {
            ReturnItem<RetRealTimeMonitor> r = new ReturnItem<RetRealTimeMonitor>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var DashBoardType = Convert.ToInt32(parameter.DashBoardType);// 2设备，3总览
                    var addmonitor = user.U_HomeConfiguration.Where(s => s.OrgID == OrgID && s.DashBoardType == DashBoardType).AsQueryable();
                    if ("2".Equals(parameter.DashBoardType)) // 设备
                    {
                        var DeviceID = Convert.ToInt32(parameter.DeviceID);
                        if ("0".Equals(parameter.ChartType))
                        {
                            addmonitor = addmonitor.Where(s => s.DeviceID == DeviceID && s.ChartType == 0);
                        }
                        else if ("1".Equals(parameter.ChartType) || "2".Equals(parameter.ChartType))
                        {
                            addmonitor = addmonitor.Where(s => s.DeviceID == DeviceID && s.DashBoardData == parameter.DashBoardData);
                        }
                    }
                    else if ("3".Equals(parameter.DashBoardType)) // 总览
                    {
                        if ("0".Equals(parameter.ChartType))
                        {
                            addmonitor = addmonitor.Where(s => s.ChartType == 0);
                        }
                        else if ("1".Equals(parameter.ChartType) || "2".Equals(parameter.ChartType))
                        {
                            addmonitor = addmonitor.Where(s => s.DashBoardData == parameter.DashBoardData);
                        }
                    }
                    addmonitor = addmonitor.OrderByDescending(s => s.CreateTime);
                    
                    if (addmonitor.ToList().Count != 0)
                    {
                        if ("1".Equals(parameter.ChartType) || "2".Equals(parameter.ChartType))// 0 背景图信息 1 设备信息 2 设备属性信息
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "该位置数据信息已存在";
                            return r;
                        }
                        else if ("0".Equals(parameter.ChartType))
                        {
                            if ("2".Equals(parameter.DashBoardType)) // 设备
                            {
                                var DeviceID = Convert.ToInt32(parameter.DeviceID);
                                var editmonitor = user.U_HomeConfiguration.Where(s => s.OrgID == OrgID && s.DashBoardType == DashBoardType && s.DeviceID == DeviceID && s.ChartType == 0).FirstOrDefault();
                                editmonitor.DashBoardData = parameter.DashBoardData;
                                editmonitor.Remark = parameter.Remark;
                                user.SaveChanges();
                            }
                            else if ("3".Equals(parameter.DashBoardType)) // 总览
                            {
                                var editmonitor = user.U_HomeConfiguration.Where(s => s.OrgID == OrgID && s.DashBoardType == DashBoardType && s.ChartType == 0).FirstOrDefault();
                                editmonitor.DashBoardData = parameter.DashBoardData;
                                editmonitor.Remark = parameter.Remark;
                                user.SaveChanges();
                            }

                            if (parameter.DelDeviceTags.Count != 0)
                            {
                                foreach (var deltags in parameter.DelDeviceTags)
                                {
                                    var delid = Convert.ToInt32(deltags);
                                    var deletechart = user.U_HomeConfiguration.Where(s => s.ID == delid).FirstOrDefault();
                                    if (deletechart != null)
                                    {
                                        var entry = user.Entry(deletechart);
                                        //设置该对象的状态为删除
                                        entry.State = EntityState.Deleted;
                                        user.SaveChanges();
                                        //保存修改
                                    }
                                }
                            }

                            if (parameter.DelTags.Count != 0)
                            {
                                foreach (var deltags in parameter.DelTags)
                                {
                                    var delid = Convert.ToInt32(deltags);
                                    var deletechart = user.U_HomeConfiguration.Where(s => s.ID == delid).FirstOrDefault();
                                    if (deletechart != null)
                                    {
                                        var entry = user.Entry(deletechart);
                                        //设置该对象的状态为删除
                                        entry.State = EntityState.Deleted;
                                        user.SaveChanges();
                                        //保存修改
                                    }
                                }
                            }

                            r.Msg = "更新成功";
                            r.Code = 0;
                            return r;
                        }
                        
                    }
                    else if (addmonitor.ToList().Count == 0)
                    {
                        U_HomeConfiguration newChart = new U_HomeConfiguration()
                        {
                            DashBoardType = DashBoardType,// 2设备，3总览
                            ChartType = Convert.ToInt32(parameter.ChartType),// 0 背景图信息 1 设备信息 2 设备属性信息
                            DashBoardData = parameter.DashBoardData,
                            Remark = parameter.Remark,
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID)
                        };
                        if (parameter.DeviceID != "" && parameter.DeviceID != null)
                        {
                            newChart.DeviceID = Convert.ToInt32(parameter.DeviceID);
                        }
                        if (parameter.DeviceItemID != "" && parameter.DeviceItemID != null)
                        {
                            newChart.DeviceItemID = Convert.ToInt32(parameter.DeviceItemID);
                        }
                        if (parameter.GroupID != "" && parameter.GroupID != null) {
                            newChart.GroupID = Convert.ToInt32(parameter.GroupID);
                        }
                        user.U_HomeConfiguration.Add(newChart);
                        user.SaveChanges();
                        r.Msg = "新增成功";
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
        /// ---------新加
        /// 实时监控页面新增数据位置
        /// </summary>
        public ReturnItem<RetRealTimeMonitor> AddRealTimeRoot(RealTimeMonitorModel parameter)
        {
            ReturnItem<RetRealTimeMonitor> r = new ReturnItem<RetRealTimeMonitor>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var DashBoardType = Convert.ToInt32(parameter.DashBoardType);// 3 实时查看的图
                    var addmonitor = user.U_HomeConfiguration.Where(s => s.OrgID == OrgID && s.DashBoardType == DashBoardType).AsQueryable();
                    //0代表背景图，1代表建筑物的名字(只有建筑物会调用这个接口)
                    //查询位置是否存在
                    addmonitor = addmonitor.Where(s => s.DashBoardData == parameter.DashBoardData);
                    addmonitor = addmonitor.OrderByDescending(s => s.CreateTime);
                    if (addmonitor.ToList().Count != 0)
                    {
                       r.Data = null;
                       r.Code = -1;
                       r.Msg = "该位置数据信息已存在";
                       return r;
                        
                    }
                    else if (addmonitor.ToList().Count == 0)
                    {
                        U_HomeConfiguration newChart = new U_HomeConfiguration()
                        {
                            DashBoardType = DashBoardType,// 2设备，3总览
                            ChartType = Convert.ToInt32(parameter.ChartType),// 0 背景图信息 1建筑物信息
                            DashBoardData = parameter.DashBoardData,
                            Remark = parameter.Remark,
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID)
                        };
                        user.U_HomeConfiguration.Add(newChart);
                        user.SaveChanges();
                        r.Msg = "新增成功";
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
        /// 获取实时监测展示数据
        /// </summary>
        /// <returns>成功返回数据List,失败返回Null.</returns>
        public ReturnItem<List<RetRealTimeMonitor>> GetRealTimeMonitorList(RealTimeMonitorModel parameter, List<RetRelTime> Info)
        {

            ReturnItem<List<RetRealTimeMonitor>> r = new ReturnItem<List<RetRealTimeMonitor>>();
            List<RetRealTimeMonitor> listinfo = new List<RetRealTimeMonitor>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getchartlist = new List<U_HomeConfiguration>();
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var DashBoardType = Convert.ToInt32(parameter.DashBoardType);
                    getchartlist = user.U_HomeConfiguration.Where(s => s.DashBoardType == DashBoardType && s.OrgID == OrgID).ToList();
                    if("2".Equals(parameter.DashBoardType) && parameter.DeviceID != null && !"".Equals(parameter.DeviceID))
                    {
                        var deviceid = Convert.ToInt32(parameter.DeviceID);
                        getchartlist = getchartlist.Where(s => s.DeviceID == deviceid).ToList();
                    }
                    if (getchartlist == null || getchartlist.Count == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备数据";
                        return r;
                    }
                    if (getchartlist != null && getchartlist.Count != 0)
                    {
                        List<U_HomeConfiguration> list = getchartlist.ToList<U_HomeConfiguration>();
                        r.Count = getchartlist.Count();
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var chartlistinfo = new RetRealTimeMonitor();
                            chartlistinfo.ID = item.ID;
                            chartlistinfo.DashBoardType = item.DashBoardType.ToString();
                            chartlistinfo.DeviceID = item.DeviceID.ToString();
                            chartlistinfo.DeviceItemID = item.DeviceItemID.ToString();

                            foreach (var DeviceInfo in Info)
                            {
                                if (DeviceInfo.ID == item.DeviceID)
                                {
                                    chartlistinfo.DeviceName = DeviceInfo.Name;
                                    foreach (var DeviceItemInfo in DeviceInfo.DeviceItems)
                                    {
                                        if (DeviceItemInfo.DeviceItemID == item.DeviceItemID)
                                        {
                                            List<RetRelTimeTag> listDtag = new List<RetRelTimeTag>();
                                            listDtag = DeviceInfo.TagList;
                                            if(listDtag != null)
                                            {
                                                chartlistinfo.TagList = listDtag;
                                            }
                                            chartlistinfo.DataConnectID = DeviceInfo.DataConnectID;
                                            chartlistinfo.DeviceItemName = DeviceItemInfo.DeviceItemName;
                                        }
                                    }
                                }
                            }

                            chartlistinfo.DashBoardData = item.DashBoardData;
                            chartlistinfo.ChartType = item.ChartType.ToString();
                            if(item.ChartType == 1 || item.ChartType == 2)//0 背景图信息 1 设备信息 2 设备属性信息
                            {
                                string[] str = item.DashBoardData.Split(',');
                                chartlistinfo.Left = str[0].ToString();
                                chartlistinfo.Top = str[1].ToString();
                                if (item.ChartType == 2){
                                    chartlistinfo.GroupID = item.GroupID.ToString();
                                }
                            }
                            chartlistinfo.Remark = item.Remark;
                            chartlistinfo.CreateTime = item.CreateTime;
                            chartlistinfo.CreateUserID = item.CreateUserID.ToString();
                            chartlistinfo.OrgID = item.OrgID.ToString();
                            listinfo.Add(chartlistinfo);
                        }
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
        /// 删除实时监测数据位置信息
        /// </summary>
        public ReturnItem<RetRealTimeMonitor> DeleteData(RealTimeMonitorModel parameter)
        {
            ReturnItem<RetRealTimeMonitor> r = new ReturnItem<RetRealTimeMonitor>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var deletechart = user.U_HomeConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (deletechart != null)
                    {
                        var entry = user.Entry(deletechart);
                        //设置该对象的状态为删除
                        entry.State = EntityState.Deleted;
                        user.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "图表删除成功";
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