using Common.quanlangJson;
using GenerSoft.IndApp.AlertPoliciesBLL;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.HistoryAlertPolicies;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.HistoryAlertPolicies;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GenerSoft.IndApp.AlertPolicies.Controllers
{
    public class HistoryAlertPoliciesController : BaseApiController
    {
        /// <summary>
        /// 获取历史报警列表记录数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetHistoryAlertPoliciesDataList(HistoryAlertPoliciesModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceMonitoringApi deviceList = new DeviceMonitoringApi();
            var deviceApi = deviceList.GetDeviceList(new GetDeviceInfoParameter());
            var list = deviceApi.Data;
            HistoryAlertPoliciesBLL device = new HistoryAlertPoliciesBLL();
            var get = device.GetHistoryAlertPoliciesDataList(parameter, list);
            return InspurJson<List<RetHistoryAlertPolicies>>(get);
        }

        /// <summary>
        /// 通过id删除历史报警列表记录数据
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DelHistoryAlertPoliciesData(HistoryAlertPoliciesModel parameter)
        {
            HistoryAlertPoliciesBLL device = new HistoryAlertPoliciesBLL();
            var get = device.DelHistoryAlertPoliciesData(parameter);
            return InspurJson<RetHistoryAlertPolicies>(get);
        }

        /// <summary>
        /// 新增历史报警策略
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult AddHistoryAlertPolicies(HistoryAlertPoliciesModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            HistoryAlertPoliciesBLL device = new HistoryAlertPoliciesBLL();
            var get = device.AddHistoryAlertPolicies(parameter);
            return InspurJson<RetHistoryAlertPolicies>(get);
        }





        /// <summary>
        /// 对设备进行控制，返回最新的设备信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult ControllDevice(command c)
        {
            ControllDeviceBLL cdb = new ControllDeviceBLL();
            var rb = cdb.ControllDevice(c);
            return InspurJson<RootObject>(rb);
        }

        /// <summary>
        /// 对设备进行批量控制，返回是否成功
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult ControllDeviceList(command c)
        {
            ControllDeviceBLL cdb = new ControllDeviceBLL();
            var rb = cdb.ControllDeviceList(c);
            return InspurJson<RootObject>(rb);
        }
        /// <summary>
        /// 获取最新的设备信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult getLastestDeviceInfo(command c)
        {
            ControllDeviceBLL cdb = new ControllDeviceBLL();
            var rb = cdb.getLastestDeviceInfoOuter(c);
            return InspurJson<RootObject>(rb);
        }
    }
}