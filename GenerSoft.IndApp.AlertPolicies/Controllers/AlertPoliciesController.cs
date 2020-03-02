using Common;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceData;
using GenerSoft.IndApp.AlertPoliciesBLL;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlertPolicies;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlertPolicies;
using System.Collections;
using Newtonsoft.Json;
using Common.Config;
using GenerSoft.IndApp.CommonSdk.Model.User;

namespace GenerSoft.IndApp.AlertPolicies.Controllers
{
    public class AlertPoliciesController : BaseApiController
    {
        /// <summary>
        /// 获取报警策略列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlertPoliciesList(AlertPoliciesModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceMonitoringApi deviceList = new DeviceMonitoringApi();
            var deviceApi = deviceList.GetDeviceList(new GetDeviceInfoParameter());
            var list = deviceApi.Data;
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var get = device.GetAlertPoliciesList(parameter, list);
            List<GetDeviceDataParameter> DataList = new List<GetDeviceDataParameter>();
            try
            {
                foreach (var item in get.Data)
                {
                    var model = new GetDeviceDataParameter();
                    model.ID = item.ID;
                    model.DeviceID = item.DeviceID;
                    model.DeviceItemID = item.DeviceItemId;
                    model.DataConnectID = item.DataConnectID;
                    model.TagMap = JsonConvert.SerializeObject(item.TagList);
                    DataList.Add(model);
                }
            }
            catch
            {

            }
            var data = deviceList.GetDeviceCurrentData(DataList);
            if (data.Data.Count> 0)
            {
                foreach (var item in get.Data)
                {
                    foreach (var returnitem in data.Data)
                    {
                        if (null == returnitem)
                        {
                            item.CurrentData = "暂无数据";
                        }
                        else if (returnitem.ID == item.ID)
                        {
                            item.CurrentData = returnitem.Value;
                        }
                    }
                }
            }
            return InspurJson<List<RetAlertPolicies>>(get);
        }

        /// <summary>
        /// 新增报警策略
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddAlertPoliciesInfo(AlertPoliciesModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var get = device.AddAlertPoliciesInfo(parameter);
            return InspurJson<RetAlertPolicies>(get);
        }
        /// <summary>
        /// 新增报警策略列表,按照查询设备模板下的所有设备进行添加
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddAlertPoliciesInfoList1(AlertPoliciesListModel parameter)
        {
            UserApi api = new UserApi();
            ReturnItem<RetAlertPolicies> get = new ReturnItem<RetAlertPolicies>();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            AlertPoliciesModel alert = new AlertPoliciesModel();
            alert.StrategyName = parameter.StrategyName;
            alert.Remark = parameter.Remark;
            alert.CreateTime = parameter.CreateTime;
            alert.CreateTime = parameter.CreateTime;
            alert.Interval = parameter.Interval;
            alert.OrgID = parameter.OrgID;
            alert.Active = parameter.Active;
            foreach (var item in parameter.DeviceList)
            {
                alert.DeviceID = item;
                foreach (var value in parameter.Property)
                {
                    alert.DataConnectID = value.DataConnectID;
                    alert.DeviceItemId = value.DeviceItemId;
                    alert.Compare = value.Compare;
                    alert.Threshold = value.Threshold;
                    alert.TagList = value.TagList;
                    get = device.AddAlertPoliciesInfo(alert);
                }
            }
            return InspurJson<RetAlertPolicies>(get);
        }
        /// <summary>
        /// 新增报警策略列表,原来浪潮写的
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddAlertPoliciesInfoList(AlertPoliciesListModel parameter)
        {
            UserApi api = new UserApi();
            ReturnItem<RetAlertPolicies> get = new ReturnItem<RetAlertPolicies>();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            AlertPoliciesModel alert = new AlertPoliciesModel();
            var i = 0;
            if (parameter.Property.Count() == 1)
            {
                alert.StrategyName = parameter.StrategyName;
                alert.DeviceID = parameter.Property[0].DeviceID;
                alert.DataConnectID = parameter.Property[0].DataConnectID;
                alert.DeviceItemId = parameter.Property[0].DeviceItemId;
                alert.Compare = parameter.Property[0].Compare;
                alert.Threshold = parameter.Property[0].Threshold;
                alert.TagList = parameter.Property[0].TagList;
                alert.Remark = parameter.Remark;
                alert.CreateTime = parameter.CreateTime;
                alert.CreateTime = parameter.CreateTime;
                alert.Interval = parameter.Interval;
                alert.OrgID = parameter.OrgID;
                alert.Active = parameter.Active;
                get = device.AddAlertPoliciesInfo(alert);
            }
            else
            {
                foreach (var item in parameter.Property)
                {
                    alert.StrategyName = parameter.StrategyName + '(' + (i + 1).ToString() + ')';
                    alert.DeviceID = item.DeviceID;
                    alert.DataConnectID = item.DataConnectID;
                    alert.DeviceItemId = item.DeviceItemId;
                    alert.Compare = item.Compare;
                    alert.Threshold = item.Threshold;
                    alert.TagList = item.TagList;
                    alert.Remark = parameter.Remark;
                    alert.CreateTime = parameter.CreateTime;
                    alert.CreateTime = parameter.CreateTime;
                    alert.Interval = parameter.Interval;
                    alert.OrgID = parameter.OrgID;
                    alert.Active = parameter.Active;
                    get = device.AddAlertPoliciesInfo(alert);
                    i++;
                }
            }
            return InspurJson<RetAlertPolicies>(get);
        }

        /// <summary>
        /// 根据ID获取报警策略信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlertPoliciesInfoByID(AlertPoliciesModel parameter)
        {
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var get = device.GetAlertPoliciesInfoByID(parameter);
            return InspurJson<RetAlertPolicies>(get);
        }

        /// <summary>
        /// 更新报警策略信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult UpdateAlertPolicies(AlertPoliciesModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UpdateUserId = Convert.ToInt32(userApi.Data.UserId);
            model.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var update = device.UpdateAlertPolicies(model);
            return InspurJson<RetAlertPolicies>(update);
        }

        /// <summary>
        /// 删除报警策略信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteAlertPolicies(AlertPoliciesModel model)
        {
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var delete = device.DeleteAlertPolicies(model);
            return InspurJson<RetAlertPolicies>(delete);
        }

        [InnerCallFilter]
        [HttpPost]
        public IHttpActionResult EnableDeviceService(EnableDeviceParmeter par)
        {
            if (CustomConfigParam.EnableMqtt)
            {
                AlertServiceBLL bll = new AlertServiceBLL();
                if (par.IsEnable)
                {
                    bll.EnableDevice(par.DeviceId);
                }
                else {
                    bll.DisableDeivce(par.DeviceId);
                }
            }
            return InspurJson<object>(new ReturnItem<object>() { Code = 0, Msg = "" }, true);

        }

        /// <summary>
        /// 获取报警策略个数
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlertPoliciesNumber(AlertPoliciesModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var get = device.GetAlertPoliciesNum(model);
            return InspurJson<string>(get);
        }

        /// <summary>
        /// 获取报警策略个数（内部调用）
        /// </summary>
        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetAlertPoliciesNum(EnableDeviceParmeter par)
        {
            AlertPoliciesModel parameter = new AlertPoliciesModel();
            UserApi api = new UserApi();
            var userApi = api.GetUserInfo(new GetUserInfoParameter() { TokenId = par.TokenID });
            parameter.OrgID = userApi.Data.OrgID.ToString();
            AlertPoliciesInfoBLL device = new AlertPoliciesInfoBLL();
            var get = device.GetAlertPoliciesNum(parameter);
            return InspurJson(get, true);
        }
    }
}