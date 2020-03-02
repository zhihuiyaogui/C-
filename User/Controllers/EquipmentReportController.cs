using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.EnergyReport;
using UserBLL.Model.Parameter.HomeConfiguration;
using UserBLL.Model.Return.EnergyReport;

namespace User.Controllers
{
    public class EquipmentReportController : BaseApiController
    {
        /// <summary>
        /// 设备能源新增/更新报表配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddEquipmentEnergyChartInfo(EquipmentEnergyModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.DashBoardType = "4";
            model.DatabaseType = "0";// 0：使用连接数据库；1：本地数据查找
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            DashBoardEnergyConfigModel EnergyConfig = new DashBoardEnergyConfigModel()
            {
                DatabaseType = model.DatabaseType,
                StartTime = model.StartTime.ToString(),
                EndTime = model.EndTime.ToString(),
                StatisticalInterval = model.StatisticalInterval,
                IntervalUnit = model.IntervalUnit,
                Type = model.Type
            };
            List<HomeDeviceInfo> HomeDeviceInfoList = new List<HomeDeviceInfo>();
            if (model.Property != null)
            {
                for (int i = 0; i < model.Property.Count; i++)
                {
                    HomeDeviceInfo HomeDeviceInfo = new HomeDeviceInfo();
                    HomeDeviceInfo.DeviceID = model.Property[i].data[0];
                    HomeDeviceInfo.DeviceItemID = model.Property[i].data[1];
                    HomeDeviceInfoList.Add(HomeDeviceInfo);
                }
            }
            EnergyConfig.HomeDeviceInfoList = HomeDeviceInfoList;
            model.ChartConfig = JsonConvert.SerializeObject(EnergyConfig);
            EquipmentReportBLL device = new EquipmentReportBLL();
            var add = device.AddChart(model);
            return InspurJson<RetEquipmentEnergyList>(add);
        }

        /// <summary>
        /// 获取设备能源数据信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEquipmentEnergyChartInfo()
        {
            EquipmentEnergyModel model = new EquipmentEnergyModel();
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID.ToString();
            model.DashBoardType = "4";
            EquipmentReportBLL device = new EquipmentReportBLL();
            var get = device.GetEquipmentEnergyChartList(model);
            return InspurJson<RetEquipmentEnergyList>(get);
        }
    }
}