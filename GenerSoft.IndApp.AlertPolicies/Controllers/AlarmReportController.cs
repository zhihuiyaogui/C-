using GenerSoft.IndApp.AlertPoliciesBLL;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Return.AlarmReport;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.AlarmReport;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace GenerSoft.IndApp.AlertPolicies.Controllers
{
    public class AlarmReportController : BaseApiController
    {
        /// <summary>
        /// 获取统计报警信息列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlarmReportList(AlarmReportModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceMonitoringApi deviceList = new DeviceMonitoringApi();
            var deviceApi = deviceList.GetDeviceList(new GetDeviceInfoParameter());
            var list = deviceApi.Data;
            AlarmReportInfoBLL device = new AlarmReportInfoBLL();
            var get = device.GetAlarmReportList(parameter, list);
            return InspurJson<List<RetAlarmReport>>(get);
        }

        /// <summary>
        /// 查看单个设备属性报警信息数据列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAlarmReportDetailDataList(AlarmReportItemModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceMonitoringApi deviceList = new DeviceMonitoringApi();
            var deviceApi = deviceList.GetDeviceList(new GetDeviceInfoParameter());
            var list = deviceApi.Data;
            AlarmReportInfoBLL device = new AlarmReportInfoBLL();
            var get = device.GetAlarmReportDetailDataList(parameter, list);
            return InspurJson<RetAlarmReportDetail>(get);
        }
    }
}