using Common;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DeviceMonitoringBLL;
using DeviceMonitoringBLL.Model.Return.DeviceData;
using DeviceMonitoringBLL.Model.Parameter.DeviceData;
using Newtonsoft.Json;
using DeviceMonitoringBLL.Model.Parameter.EnergyReport;
using DeviceMonitoringBLL.Model.Return.EnergyReport;
using DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring;

namespace DeviceMonitoring.Controllers
{
    public class EquipmentDataController : BaseApiController
    {
        


        /// <summary>
        /// 获取设备数据
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetData()
        {
            DeviceDataBLL data = new DeviceDataBLL();
            var get = data.GetData();
            return InspurJson<RetDeviceTableData>(get);
        }

        
        /// <summary>
        /// 获取设备图表数据
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetEquipmentChartData(DeviceChartDataListModel model)
        {
            ReturnItem<RetDeviceChartData> get = new ReturnItem<RetDeviceChartData>();

            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID;

            DeviceDataBLL data = new DeviceDataBLL();
            DeviceChartDataModel parameter = new DeviceChartDataModel()
            {
                ChartType = model.ChartType,
                DatabaseType = model.DatabaseType,
                ValueType = model.ValueType,
                DataType = model.DataType,
                RecentInterval = model.RecentInterval,
                RecentUnit = model.RecentUnit,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                OrgID = model.OrgID
            };
            if (model.DatabaseType == "0")
            {
                // 折线图、柱状图多属性
                if (model.DeviceInfo.Count > 1)
                {
                    List<RetDeviceChartData> devicedata = new List<RetDeviceChartData>();
                    RetDeviceChartData info = new RetDeviceChartData();
                    List<string> Legend = new List<string>();
                    List<string> Time = new List<string>();
                    List<DeviceMultipleData> MultipleData = new List<DeviceMultipleData>();
                    foreach (var item in model.DeviceInfo)
                    {
                        parameter.DeviceID = item.DeviceID;
                        parameter.DeviceItemID = item.DeviceItemID;
                        get = data.GetEquipmentChartData(parameter);
                        if (get.Data != null && get.Data.Data.Count != 0)
                        {
                            devicedata.Add(get.Data);
                        }
                    }
                    devicedata = devicedata.OrderBy(o => Convert.ToDateTime(o.Time[0])).ToList();
                    foreach (var item in devicedata)
                    {
                        if (item.Data.Count != 0)
                        {
                            Legend.Add(item.DeviceName + "/" + item.DeviceItemName);
                        }

                        DeviceMultipleData pra = new DeviceMultipleData();
                        pra.DeviceName = item.DeviceName;
                        pra.DeviceItemName = item.DeviceItemName;
                        List<List<object>> singledata = new List<List<object>>();
                        for (var j = 0; j < item.Time.Count; j++)
                        {
                            if (Time.Contains(item.Time[j]) == false)
                            {
                                Time.Add(item.Time[j]);
                            }
                            List<object> single = new List<object>();
                            single.Add(item.Time[j]);
                            single.Add(item.Data[j]);
                            singledata.Add(single);
                        }
                        pra.DeviceData = singledata;
                        MultipleData.Add(pra);
                    }
                    info.Time = Time.OrderBy(o => DateTime.Parse(o)).ToList();
                    info.Legend = Legend;
                    info.MultipleData = MultipleData;

                    get.Msg = "设备数据获取成功";
                    get.Code = 0;
                    get.Data = info;
                }
                // 单个设备属性
                else if (model.DeviceInfo.Count == 1)
                {
                    parameter.DeviceID = model.DeviceInfo[0].DeviceID;
                    parameter.DeviceItemID = model.DeviceInfo[0].DeviceItemID;
                    get = data.GetEquipmentChartData(parameter);
                }
                
            }
            else if (model.DatabaseType == "1")
            {
                get = data.GetEquipmentChartData(parameter);
            }
            return InspurJson<RetDeviceChartData>(get);
        }

        /// <summary>
        /// 获取设备运行报表属性图表数据
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult GetEquipmentReportListData(DeviceDataModel model)
        {
            DeviceDataBLL data = new DeviceDataBLL();
            var get = data.GetEquipmentReportListData(model);
            return InspurJson<RetDeviceTableData>(get);
        }

        /// <summary>
        /// 获取处理后的设备运行报表属性图表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEquipmentReportListDataByHandl(DeviceDataListModel model)
        {
            DeviceDataBLL data = new DeviceDataBLL();
            RetDeviceTableData tabledata = new RetDeviceTableData();
            ReturnItem<List<RetDeviceTableData>> retdata = new ReturnItem<List<RetDeviceTableData>>();
            DeviceDataModel datamodel = new DeviceDataModel()
            {
                DeviceID = model.DeviceID,
                EndTime = model.EndTime,
                StartTime = model.StartTime,
                GetTogetherType = model.GetTogetherType,
                IntervalUnit = model.IntervalUnit,
                StatisticalInterval = model.StatisticalInterval
            };

            if (model.DeviceItemIDs == null || model.DeviceItemIDs.Count == 0)//当前台不选属性时，将该设备所有的属性展示
            {
                UserApi api = new UserApi();
                var userApi = api.GetUserInfoByToken();
                var deviceid = 0;
                if (model.DeviceID != null && model.DeviceID != "")
                {
                    deviceid = Convert.ToInt32(model.DeviceID);
                }
                DeviceItemInfoModel parameter = new DeviceItemInfoModel();
                parameter.OrgID = userApi.Data.OrgID.ToString();
                parameter.DeviceID = deviceid;

                DeviceInfoBLL device = new DeviceInfoBLL();
                var getitem = device.GetEquipmentItemsList(parameter);
                if (getitem.Data != null)
                {
                    foreach (var item in getitem.Data)
                    {
                        model.DeviceItemIDs.Add(item.ID);//核心语句
                    }
                }
                else
                {
                    return null;
                }

            }

            if (model.DeviceItemIDs != null && model.DeviceItemIDs.Count > 0)
            {
                var count = 0;
                List<RetDeviceTableList> listtabledata = new List<RetDeviceTableList>();
                List<string> listtimedata = new List<string>();
                List<List<object>> listcordata = new List<List<object>>();
                List<List<object>> listanodata = new List<List<object>>();
                List<List<object>> listnordata = new List<List<object>>();
                List<double> listdatadata = new List<double>();

                List<RetDeviceTableData> listdata = new List<RetDeviceTableData>();
                foreach (var item in model.DeviceItemIDs)
                {
                    RetDeviceTableData chartdata = new RetDeviceTableData();//图表数据
                    datamodel.DeviceItemID = item;
                    var get = data.GetEquipmentReportListDataByHandl(datamodel);
                    if(get != null && get.Data != null)
                    {
                        chartdata.AllCoordination = get.Data.AllCoordination;
                        chartdata.AnomCoordination = get.Data.AnomCoordination;
                        chartdata.Data = get.Data.Data;
                        chartdata.DeviceItemName = get.Data.DeviceItemName;
                        chartdata.DeviceName = get.Data.DeviceName;
                        chartdata.DeviceTableList = get.Data.DeviceTableList;
                        chartdata.NormCoordination = get.Data.NormCoordination;
                        chartdata.Time = get.Data.Time;
                        chartdata.Unit = get.Data.Unit;
                        chartdata.type = "2";
                        listdata.Add(chartdata);


                        if (get.Data.DeviceTableList != null)
                        {
                            foreach (var listitem in get.Data.DeviceTableList)
                            {
                                listtabledata.Add(listitem);
                            }
                        }
                        if (get.Data.Time != null)
                        {
                            foreach (var listtime in get.Data.Time)
                            {
                                listtimedata.Add(listtime);
                            }
                        }
                        if (get.Data.AllCoordination != null)
                        {
                            foreach (var cooitem in get.Data.AllCoordination)
                            {
                                listcordata.Add(cooitem);
                            }
                        }
                        if (get.Data.AnomCoordination != null)
                        {
                            foreach (var anoitem in get.Data.AnomCoordination)
                            {
                                listanodata.Add(anoitem);
                            }
                        }
                        if (get.Data.NormCoordination != null)
                        {
                            foreach (var noritem in get.Data.NormCoordination)
                            {
                                listnordata.Add(noritem);
                            }
                        }
                        if (get.Data.Data != null)
                        {
                            foreach (var dataitem in get.Data.Data)
                            {
                                listdatadata.Add(dataitem);
                            }
                        }
                        count += get.Count;
                        retdata.Code = get.Code;
                        retdata.Msg = get.Msg;
                    }
                }
                tabledata.DeviceTableList = listtabledata;
                tabledata.Time = listtimedata;
                tabledata.AllCoordination = listcordata;
                tabledata.AnomCoordination = listanodata;
                tabledata.NormCoordination = listnordata;
                tabledata.Data = listdatadata;
                tabledata.type = "1";

                listdata.Add(tabledata);//添加列表数据
                retdata.Data = listdata;
                retdata.Count = count;
            }
            //else
            //{
            //    datamodel.DeviceItemID = model.DeviceItemIDs[0];
            //    var get = data.GetEquipmentReportListDataByHandl(datamodel);
            //    retdata = get;
            //}
            return InspurJson <List<RetDeviceTableData>>(retdata);
        }

        /// <summary>
        /// 获取处理后的设备运行报表属性图表数据,用于数据对比时使用。传入多个设备和单个属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]

        public IHttpActionResult GetEquipmentReportListDataByHandl2(DeviceDataListModel2 model)
        {
            DeviceDataBLL data = new DeviceDataBLL();
            RetDeviceTableData tabledata = new RetDeviceTableData();
            ReturnItem<List<RetDeviceTableData>> retdata = new ReturnItem<List<RetDeviceTableData>>();
            DeviceDataModel datamodel = new DeviceDataModel()
            {
                EndTime = model.EndTime,
                StartTime = model.StartTime,
                GetTogetherType = model.GetTogetherType,
                IntervalUnit = model.IntervalUnit,
                StatisticalInterval = model.StatisticalInterval
            };

            Dictionary<string, string> itemIdToDeviceId = new Dictionary<string, string>();//key为itemID，value为deviceId
            if (model.DeviceIDs != null || model.DeviceIDs.Count != 0) //获取参数中的所有设备对应的propertyLable属性
            {
                UserApi api = new UserApi();
                var userApi = api.GetUserInfoByToken();
                model.DeviceItemIDs = new List<string>();
                foreach (var id in model.DeviceIDs)
                {
                    var deviceid = Convert.ToInt32(id);
                    DeviceItemInfoModel parameter = new DeviceItemInfoModel();
                    parameter.OrgID = userApi.Data.OrgID.ToString();
                    parameter.DeviceID = deviceid;

                    DeviceInfoBLL device = new DeviceInfoBLL();
                    var getitem = device.GetEquipmentItemsList(parameter);
                    if (getitem.Data != null)
                    {
                        var temp = getitem.Data;
                        foreach (var item in getitem.Data)
                        {
                            if(item.PropertyLabel == model.DeviceItemPropertyLabel)//如果是该PropertyLabel的，则放到参数里面
                            {
                                model.DeviceItemIDs.Add(item.ID);
                                itemIdToDeviceId.Add(item.ID, id);
                                break;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (model.DeviceItemIDs != null && model.DeviceItemIDs.Count > 0)
            {
                var count = 0;
                List<RetDeviceTableList> listtabledata = new List<RetDeviceTableList>();
                List<string> listtimedata = new List<string>();
                List<List<object>> listcordata = new List<List<object>>();
                List<List<object>> listanodata = new List<List<object>>();
                List<List<object>> listnordata = new List<List<object>>();
                List<double> listdatadata = new List<double>();

                List<RetDeviceTableData> listdata = new List<RetDeviceTableData>();
                foreach (var item in model.DeviceItemIDs)
                {
                    RetDeviceTableData chartdata = new RetDeviceTableData();//图表数据
                    datamodel.DeviceItemID = item;
                    string id = "";
                    itemIdToDeviceId.TryGetValue(item,out id);
                    if (id == null||id == "")
                    {
                        retdata.Code = -1;
                        retdata.Msg = "在Dictionary中获取deviceId失败";
                        retdata.Data = null;
                        return InspurJson<List<RetDeviceTableData>>(retdata);
                    }
                    datamodel.DeviceID = id;
                    var get = data.GetEquipmentReportListDataByHandl(datamodel);
                    if (get != null && get.Data != null)
                    {
                        chartdata.AllCoordination = get.Data.AllCoordination;
                        chartdata.AnomCoordination = get.Data.AnomCoordination;
                        chartdata.Data = get.Data.Data;
                        chartdata.DeviceItemName = get.Data.DeviceItemName;
                        chartdata.DeviceName = get.Data.DeviceName;
                        chartdata.DeviceTableList = get.Data.DeviceTableList;
                        chartdata.NormCoordination = get.Data.NormCoordination;
                        chartdata.Time = get.Data.Time;
                        chartdata.Unit = get.Data.Unit;
                        chartdata.type = "2";
                        listdata.Add(chartdata);


                        if (get.Data.DeviceTableList != null)
                        {
                            foreach (var listitem in get.Data.DeviceTableList)
                            {
                                listtabledata.Add(listitem);
                            }
                        }
                        if (get.Data.Time != null)
                        {
                            foreach (var listtime in get.Data.Time)
                            {
                                listtimedata.Add(listtime);
                            }
                        }
                        if (get.Data.AllCoordination != null)
                        {
                            foreach (var cooitem in get.Data.AllCoordination)
                            {
                                listcordata.Add(cooitem);
                            }
                        }
                        if (get.Data.AnomCoordination != null)
                        {
                            foreach (var anoitem in get.Data.AnomCoordination)
                            {
                                listanodata.Add(anoitem);
                            }
                        }
                        if (get.Data.NormCoordination != null)
                        {
                            foreach (var noritem in get.Data.NormCoordination)
                            {
                                listnordata.Add(noritem);
                            }
                        }
                        if (get.Data.Data != null)
                        {
                            foreach (var dataitem in get.Data.Data)
                            {
                                listdatadata.Add(dataitem);
                            }
                        }
                        count += get.Count;
                        retdata.Code = get.Code;
                        //retdata.Msg = get.Msg;
                        retdata.Msg = "wahahaaaaaaaaa";
                    }
                }
                tabledata.DeviceTableList = listtabledata;
                tabledata.Time = listtimedata;
                tabledata.AllCoordination = listcordata;
                tabledata.AnomCoordination = listanodata;
                tabledata.NormCoordination = listnordata;
                tabledata.Data = listdatadata;
                tabledata.type = "1";

                listdata.Add(tabledata);//添加列表数据
                retdata.Data = listdata;
                retdata.Count = count;
            }
            return InspurJson<List<RetDeviceTableData>>(retdata);
        }

        /// <summary>
        /// 实时报警图表获取最新一条数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlarmDetailData(DeviceDataModel model)
        {
            DeviceDataBLL data = new DeviceDataBLL();
            var get = data.GetCurrentData(model);
            return InspurJson<RetDeviceTableData>(get);
        }

        /// <summary>
        /// 实时监测获取最新一条数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEquipmentMonitorData(List<DeviceDataModel> model)
        {
            ReturnItem<List<RetDeviceCurrentData>> r = new ReturnItem<List<RetDeviceCurrentData>>();
            List<RetDeviceCurrentData> listinfo = new List<RetDeviceCurrentData>();
            DeviceDataBLL data = new DeviceDataBLL();
            foreach (var item in model) {
                var get = data.GetCurrentData(item);
                RetDeviceCurrentData info = new RetDeviceCurrentData();
                if (get.Data == null)
                {
                    info.DeviceId = item.DeviceID;
                    info.DeviceItemId = item.DeviceItemID;
                    info.Value = "暂无数据";
                }
                else if (get.Data != null && get.Data.Data.Count > 0)
                {
                    info.DeviceId = item.DeviceID;
                    info.DeviceItemId = item.DeviceItemID;
                    info.Value = get.Data.Data[0].ToString();
                }
                //data.setAirRank(info);
                listinfo.Add(info);
            }
            data.setAirRank(listinfo);
            r.Data = listinfo;
            r.Msg = "数据获取成功";
            r.Code = 0;
            return InspurJson<List<RetDeviceCurrentData>>(r);
        }

        /// <summary>
        /// 获取实时数据信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetLastData(ResponseLastData model)
        {
            DeviceDataBLL data = new DeviceDataBLL();
            var get = data.GetLastData(model);
            return InspurJson<List<RetLastData>>(get);
        }

        /// <summary>
        /// 获取能源报表列表数据
        /// </summary>
      
        [HttpPost]
        public IHttpActionResult GetEnergyReportListData(EquipmentEnergyModel parameter)
        {
            RetDeviceTableData info = new RetDeviceTableData();
            List<RetDeviceTableList> table = new List<RetDeviceTableList>();
            if (parameter.Property != null)
            {
                int i = 0;
                for (; i < parameter.Property.Count; i++)
                {
                    DeviceDataModel model = new DeviceDataModel();
                    model.DeviceID = parameter.Property[i].data[0];
                    model.DeviceItemID = parameter.Property[i].data[1];
                    model.StartTime = parameter.StartTime;
                    model.EndTime = parameter.EndTime;
                    model.StatisticalInterval = parameter.StatisticalInterval;
                    model.IntervalUnit = parameter.IntervalUnit;
                    model.GetTogetherType = parameter.Type;
                    DeviceDataBLL data = new DeviceDataBLL();
                    var get = data.GetEnergyReportListData(model);
                    if (get.Data != null)
                    {
                        foreach (var item in get.Data.DeviceTableList)
                        {
                            table.Add(item);
                        }
                    }
                }
            }
            info.DeviceTableList = table.OrderByDescending(o => o.Time).ToList();
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            r.Count = info.DeviceTableList.Count();
            r.Msg = "设备数据获取成功";
            r.Code = 0;
            r.Data = info;
            return InspurJson<RetDeviceTableData>(r);
        }

        /// <summary>
        /// 获取能源报表列表数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEnergyReportChartData(EquipmentEnergyModel parameter)
        {
            List<RetDeviceTableData> devicedata = new List<RetDeviceTableData>();
            RetEquipmentEnergyChart info = new RetEquipmentEnergyChart();
            List<string> LineLegend = new List<string>();
            List<string> PieLegend = new List<string>();
            List<string> Time = new List<string>();
            List<EquipmentEnergyLine> LineData = new List<EquipmentEnergyLine>();
            List<EquipmentEnergyPie> PieList = new List<EquipmentEnergyPie>();
            if (parameter.Property != null)
            {
                int i = 0;
                for (; i < parameter.Property.Count; i++)
                {
                    DeviceDataModel model = new DeviceDataModel();
                    model.DeviceID = parameter.Property[i].data[0];
                    model.DeviceItemID = parameter.Property[i].data[1];
                    model.StartTime = parameter.StartTime;
                    model.EndTime = parameter.EndTime;
                    model.StatisticalInterval = parameter.StatisticalInterval;
                    model.IntervalUnit = parameter.IntervalUnit;
                    model.GetTogetherType = parameter.Type;
                    DeviceDataBLL data = new DeviceDataBLL();
                    var get = data.GetEnergyReportListData(model);
                    if (get.Data != null && get.Data.Data.Count != 0) {
                        devicedata.Add(get.Data);
                    }
                }
            }
            devicedata = devicedata.OrderBy(o => Convert.ToDateTime(o.Time[0])).ToList();
            foreach (var item in devicedata)
            { 
                if (item.Data.Count != 0)
                {
                    LineLegend.Add(item.DeviceName + "/" + item.DeviceItemName);
                    PieLegend.Add(item.DeviceName + "/" + item.DeviceItemName);
                }

                EquipmentEnergyLine line = new EquipmentEnergyLine();
                line.DeviceName = item.DeviceName;
                line.DeviceItemName = item.DeviceItemName;
                List<List<object>> singledata = new List<List<object>>();
                for (var j = 0; j < item.Time.Count; j++)
                {
                    if (Time.Contains(item.Time[j]) == false)
                    {
                        Time.Add(item.Time[j]);
                    }
                    List<object> single = new List<object>();
                    single.Add(item.Time[j]);
                    single.Add(item.Data[j]);
                    singledata.Add(single);
                }
                line.DeviceData = singledata;
                LineData.Add(line);

                EquipmentEnergyPie pie = new EquipmentEnergyPie();
                if (item.Data.Count != 0)
                {
                    double value = 0;
                    for (var k = 0; k < item.Data.Count; k++)
                    {
                        if (k == 0)
                        {
                            value = item.Data[k];
                        }
                        else
                        {
                            value += item.Data[k];
                        }
                    }
                    pie.value = value.ToString();
                    pie.name = item.DeviceName + "/" + item.DeviceItemName;
                }
                PieList.Add(pie);
            }
            info.Time = Time.OrderBy(o => DateTime.Parse(o)).ToList();
            info.LineLegend = LineLegend;
            info.PieLegend = PieLegend;
            info.LineData = LineData;
            info.PieList = PieList;
            ReturnItem<RetEquipmentEnergyChart> r = new ReturnItem<RetEquipmentEnergyChart>();
            r.Msg = "设备数据获取成功";
            r.Code = 0;
            r.Data = info;
            return InspurJson<RetEquipmentEnergyChart>(r);
        }

        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetDeviceCurrentDataInside(GetDeviceDataParameter parameter)
        {
            ReturnItem<List<RetDeviceCurrentData>> r = new ReturnItem<List<RetDeviceCurrentData>>();
            List<RetDeviceCurrentData> listinfo = new List<RetDeviceCurrentData>();
            List<DeviceDataInfoModel> Devicelist = new List<DeviceDataInfoModel>();
            if (parameter.DeviceInfo != "" && parameter.DeviceInfo != null)
            {
                Devicelist = JsonConvert.DeserializeObject<List<DeviceDataInfoModel>>(parameter.DeviceInfo);
            }
            DeviceDataBLL device = new DeviceDataBLL();
            foreach (var item in Devicelist)
            {
                var ReturnData = device.GetDeviceCurrentDataInside(item);
                if (ReturnData.Code == -1)
                {
                    continue;
                }
                listinfo.Add(ReturnData.Data);
            }
            r.Data = listinfo;
            r.Msg = "数据获取成功";
            r.Code = 0;
            return InspurJson(r, true);
        }
    }
}