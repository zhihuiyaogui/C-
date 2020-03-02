using Common;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceData;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.WebApiFilterAttr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.HomeConfiguration;
using UserBLL.Model.Return.HomeConfiguration;

namespace User.Controllers
{
    public class HomeConfigurationController : BaseApiController
    {
        /// <summary>
        /// 获取图表列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetChartList(HomeConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var get = device.GetChartList(model);
            return InspurJson<List<RetHomeConfiguration>>(get);
        }

        /// <summary>
        /// 利用ID获取图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetChart(HomeConfigurationModel model)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var get = device.GetChart(model);
            return InspurJson<RetHomeConfiguration>(get);
        }

        /// <summary>
        /// 验证图表是否存在
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult CheckChart(List<HomeConfigurationModel> list)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var get = device.CheckChart(list);
            return InspurJson<RetHomeConfiguration>(get);
        }

        /// <summary>
        /// 首页新增图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult IndexAddChart(HomeConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.DashBoardType = "0";
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            DashBoardDataListModel DashBoardData = new DashBoardDataListModel()
            {
                x = 0,
                y = 0,
                w = Convert.ToInt32(model.Width),
                h = Convert.ToInt32(model.Height)
            };
            model.DashBoardData = JsonConvert.SerializeObject(DashBoardData);

            DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel()
            {
                DatabaseType = model.DatabaseType,
                DataType = model.DataType,
                RecentInterval = model.RecentInterval,
                RecentUnit = model.RecentUnit,
                StartTime = model.StartTime.ToString(),
                EndTime = model.EndTime.ToString(),
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                ValueType = model.ValueType,
                TextColor = model.TextColor,
                BackgroundColor = model.BackgroundColor
            };
            List<HomeDeviceInfo> HomeDeviceInfoList = new List<HomeDeviceInfo>();
            if (model.Property != null)
            {
                if (model.Property.Count == 1)//设备,总览,表格
                {
                    if (model.Property[0].data[0] == "1")
                    {
                        // 设备
                        ChartConfig.SelectionType = "1";
                        HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                        HomeDeviceInfo.DeviceID = model.Property[0].data[1];
                        HomeDeviceInfo.DeviceItemID = model.Property[0].data[2];
                        HomeDeviceInfoList.Add(HomeDeviceInfo);
                    }
                    else if (model.Property[0].data[0] == "2")
                    {
                        // 总览
                        ChartConfig.SelectionType = "2";
                    }
                    else if (model.Property[0].data[0] == "3")
                    {
                        // 表格
                        ChartConfig.SelectionType = "3";
                    }
                }
                else if (model.Property.Count > 1)//只可能设备
                {
                    ChartConfig.SelectionType = "1";
                    for (int i = 0; i < model.Property.Count; i++)
                    {
                        HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                        HomeDeviceInfo.DeviceID = model.Property[i].data[1];
                        HomeDeviceInfo.DeviceItemID = model.Property[i].data[2];
                        HomeDeviceInfoList.Add(HomeDeviceInfo);
                    }
                }
            }
            ChartConfig.HomeDeviceInfoList = HomeDeviceInfoList;
            model.ChartConfig = JsonConvert.SerializeObject(ChartConfig);

            var add = device.AddChart(model);
            return InspurJson<RetHomeConfiguration>(add);
        }

        /// <summary>
        /// 首页更新图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult IndexEditChart(HomeConfigurationModel model)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            model.DashBoardType = "0";//首页
            DashBoardDataListModel DashBoardData = new DashBoardDataListModel()
            {
                x = Convert.ToInt32(model.XAxis),
                y = Convert.ToInt32(model.YAxis),
                w = Convert.ToInt32(model.Width),
                h = Convert.ToInt32(model.Height),
                i = model.ID.ToString()
            };
            model.DashBoardData = JsonConvert.SerializeObject(DashBoardData);

            DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel()
            {
                DatabaseType = model.DatabaseType,
                DataType = model.DataType,
                RecentInterval = model.RecentInterval,
                RecentUnit = model.RecentUnit,
                StartTime = model.StartTime.ToString(),
                EndTime = model.EndTime.ToString(),
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                ValueType = model.ValueType,
                TextColor = model.TextColor,
                BackgroundColor = model.BackgroundColor
            };
            List<HomeDeviceInfo> HomeDeviceInfoList = new List<HomeDeviceInfo>();
            if (model.Property != null)
            {
                if (model.Property.Count == 1) //设备,总览,表格
                {
                    if (model.Property[0].data[0] == "1")
                    {
                        // 设备
                        ChartConfig.SelectionType = "1";
                        HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                        HomeDeviceInfo.DeviceID = model.Property[0].data[1];
                        HomeDeviceInfo.DeviceItemID = model.Property[0].data[2];
                        HomeDeviceInfoList.Add(HomeDeviceInfo);
                    }
                    else if (model.Property[0].data[0] == "2")
                    {
                        // 总览
                        ChartConfig.SelectionType = "2";
                    }
                    else if (model.Property[0].data[0] == "3")
                    {
                        // 表格
                        ChartConfig.SelectionType = "3";
                    }
                }
                else if (model.Property.Count > 1) //只可能设备
                {
                    for (int i = 0; i < model.Property.Count; i++)
                    {
                        ChartConfig.SelectionType = "1";
                        HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                        HomeDeviceInfo.DeviceID = model.Property[i].data[1];
                        HomeDeviceInfo.DeviceItemID = model.Property[i].data[2];
                        HomeDeviceInfoList.Add(HomeDeviceInfo);
                    }
                }
            }
            ChartConfig.HomeDeviceInfoList = HomeDeviceInfoList;
            model.ChartConfig = JsonConvert.SerializeObject(ChartConfig);

            var update = device.EditChart(model);
            return InspurJson<RetHomeConfiguration>(update);
        }

        /// <summary>
        /// 设备新增图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeviceAddChart(HomeConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.DashBoardType = "1";
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            DashBoardDataListModel DashBoardData = new DashBoardDataListModel()
            {
                x = 0,
                y = 0,
                w = Convert.ToInt32(model.Width),
                h = Convert.ToInt32(model.Height)
            };
            model.DashBoardData = JsonConvert.SerializeObject(DashBoardData);
            model.DatabaseType = "0";//默认设备源
            DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel()
            {
                DatabaseType = model.DatabaseType,
                DataType = model.DataType,
                RecentInterval = model.RecentInterval,
                RecentUnit = model.RecentUnit,
                StartTime = model.StartTime.ToString(),
                EndTime = model.EndTime.ToString(),
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                ValueType = model.ValueType,
                TextColor = model.TextColor,
                BackgroundColor = model.BackgroundColor
            };
            List<HomeDeviceInfo> HomeDeviceInfoList = new List<HomeDeviceInfo>();
            if (model.DeviceItemList != null)
            {
                for (int i = 0; i < model.DeviceItemList.Count; i++)
                {
                    HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                    HomeDeviceInfo.DeviceID = model.DeviceID;
                    HomeDeviceInfo.DeviceItemID = model.DeviceItemList[i].DeviceItemID;
                    HomeDeviceInfoList.Add(HomeDeviceInfo);
                }
            }
            ChartConfig.HomeDeviceInfoList = HomeDeviceInfoList;
            model.ChartConfig = JsonConvert.SerializeObject(ChartConfig);

            var add = device.AddChart(model);
            return InspurJson<RetHomeConfiguration>(add);
        }

        /// <summary>
        /// 设备更新图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult EditChart(HomeConfigurationModel model)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            DashBoardDataListModel DashBoardData = new DashBoardDataListModel()
            {
                x = Convert.ToInt32(model.XAxis),
                y = Convert.ToInt32(model.YAxis),
                w = Convert.ToInt32(model.Width),
                h = Convert.ToInt32(model.Height),
                i = model.ID.ToString()
            };
            model.DashBoardData = JsonConvert.SerializeObject(DashBoardData);
            model.DatabaseType = "0";//默认设备源
            DashBoardChartConfigModel ChartConfig = new DashBoardChartConfigModel()
            {
                DatabaseType = model.DatabaseType,
                DataType = model.DataType,
                RecentInterval = model.RecentInterval,
                RecentUnit = model.RecentUnit,
                StartTime = model.StartTime.ToString(),
                EndTime = model.EndTime.ToString(),
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                ValueType = model.ValueType,
                TextColor = model.TextColor,
                BackgroundColor = model.BackgroundColor
            };
            List<HomeDeviceInfo> HomeDeviceInfoList = new List<HomeDeviceInfo>();
            if (model.DeviceItemList != null)
            {
                for (int i = 0; i < model.DeviceItemList.Count; i++)
                {
                    HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                    HomeDeviceInfo.DeviceID = model.DeviceID;
                    HomeDeviceInfo.DeviceItemID = model.DeviceItemList[i].DeviceItemID;
                    HomeDeviceInfoList.Add(HomeDeviceInfo);
                }
            }
            ChartConfig.HomeDeviceInfoList = HomeDeviceInfoList;
            model.ChartConfig = JsonConvert.SerializeObject(ChartConfig);

            var update = device.EditChart(model);
            return InspurJson<RetHomeConfiguration>(update);
        }

        /// <summary>
        /// 删除图表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteChart(HomeConfigurationModel model)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var delete = device.DeleteChart(model);
            return InspurJson<RetHomeConfiguration>(delete);
        }

        /// <summary>
        /// 保存布局
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult SaveLayout(List<DashBoardDataListModel> list)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var save = device.SaveLayout(list);
            return InspurJson<RetHomeConfiguration>(save);
        }

        /// <summary>
        /// 查询设备图表名称是否存在
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult CheckDeviceInfoByChartName(HomeConfigurationModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var get = device.CheckDeviceInfoByChartName(parameter);
            return InspurJson<RetHomeConfiguration>(get);
        }

        /// <summary>
        /// 实时监控页面新增数据位置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddRealTimeMonitor(RealTimeMonitorModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            if ("1".Equals(model.ChartType))// 0 背景图信息 1 设备信息 2 设备属性信息
            {
                model.DashBoardData = Math.Round(Convert.ToDouble(model.clickX) / Convert.ToDouble(model.mainWidth), 3) * 100 + "," + Math.Round(Convert.ToDouble(model.clickY) / Convert.ToDouble(model.mainHeight), 3) * 100;
            }
            else if ("2".Equals(model.ChartType)) {
                model.DashBoardData = Math.Round(Convert.ToDouble(model.clickX) / Convert.ToDouble(model.mainWidth), 3) * 100 + "," + Math.Round(Convert.ToDouble(model.clickY) / Convert.ToDouble(model.mainHeight), 3) * 100;
            }
            else if ("0".Equals(model.ChartType))
            {
                model.DashBoardData = model.bgUrl;
            }
            var add = device.AddRealTimeMonitor(model);
            return InspurJson<RetRealTimeMonitor>(add);
        }

        /// <summary>
        /// -----新加的---------
        /// 添加一级目录的信息
        /// </summary>
        public IHttpActionResult AddRealTimeRoot(RealTimeMonitorModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            model.DashBoardData = Math.Round(Convert.ToDouble(model.clickX) / Convert.ToDouble(model.mainWidth), 3) * 100 + "," + Math.Round(Convert.ToDouble(model.clickY) / Convert.ToDouble(model.mainHeight), 3) * 100;
            var add = device.AddRealTimeRoot(model);
            return InspurJson<RetRealTimeMonitor>(add);
        }

        /// <summary>
        /// 获取监控界面展示数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetRealTimeMonitorList(RealTimeMonitorModel model)
        {
            List<GetDeviceDataParameter> list = new List<GetDeviceDataParameter>();
            List<RetRelTime> listRel = new List<RetRelTime>();
            DeviceMonitoringApi device = new DeviceMonitoringApi();
            var deviceList = device.GetDeviceList(new GetDeviceInfoParameter());
            var devicelist = deviceList.Data;

            if(devicelist != null)
            {
                foreach (var i in devicelist)
                {
                    var deviceParam = new RetRelTime();
                    deviceParam.ID = i.ID;
                    deviceParam.Name = i.Name;
                    if (i.TagList.Count != 0)
                    {
                        var tag = new RetRelTimeTag();
                        tag.Key = i.TagList[0].Key;
                        tag.Value = i.TagList[0].Value;
                        List<RetRelTimeTag> listtag = new List<RetRelTimeTag>();
                        listtag.Add(tag);
                        deviceParam.TagList = listtag;
                    }
                    List<RetDeviceItems> deviceItems = new List<RetDeviceItems>();
                    foreach(var iteminfo in i.DeviceItems)
                    {
                        RetDeviceItems items = new RetDeviceItems();
                        items.DeviceItemID = iteminfo.ID;
                        items.DeviceItemName = iteminfo.Name;
                        deviceItems.Add(items);
                    }
                    deviceParam.DeviceItems = deviceItems;
                    deviceParam.DataConnectID = i.DataConnectID;
                    listRel.Add(deviceParam);
                }
            }

            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID.ToString();
            HomeConfigurationBLL config = new HomeConfigurationBLL();
            var get = config.GetRealTimeMonitorList(model, listRel);

            if (get.Data != null)
            {
                foreach (var item in get.Data)
                {
                    if(item.ChartType == "2")
                    {
                        var param = new GetDeviceDataParameter();
                        param.DeviceID = item.DeviceID;
                        param.DeviceItemID = item.DeviceItemID;
                        param.DataConnectID = item.DataConnectID;
                        param.TagMap = JsonConvert.SerializeObject(item.TagList);
                        list.Add(param);
                    }
                }
            }
            var deviceApi = device.GetDeviceCurrentData(list);


            if (deviceApi.Data != null && deviceApi.Data.Count > 0)
            {
                foreach (var item in get.Data)
                {
                    foreach (var returnitem in deviceApi.Data)
                    {
                        if (item != null && returnitem != null)
                        {
                            if(item.DeviceItemID.Equals(returnitem.DeviceItemId))
                            {
                                if(returnitem.Value != null && !"".Equals(returnitem.Value))
                                {
                                    item.Value = returnitem.Value;
                                }
                                else
                                {
                                    item.Value = "暂无数据";
                                }
                            }
                        }
                    }
                }
            }
            return InspurJson<List<RetRealTimeMonitor>>(get);
        }

        /// <summary>
        /// 删除实时监测位置数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteData(RealTimeMonitorModel model)
        {
            HomeConfigurationBLL device = new HomeConfigurationBLL();
            var delete = device.DeleteData(model);
            return InspurJson<RetRealTimeMonitor>(delete);
        }
    }
}