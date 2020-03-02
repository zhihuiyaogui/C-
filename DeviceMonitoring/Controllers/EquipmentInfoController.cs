using Common;
using DeviceMonitoringBLL;
using DeviceMonitoringBLL.Model.Parameter.DeviceData;
using DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring;
using DeviceMonitoringBLL.Model.Return.DeviceData;
using DeviceMonitoringBLL.Model.Return.DeviceMonitoring;
using DeviceMonitoringDAL.Model.Parameter.DeviceMonitoring;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Alert;
using GenerSoft.IndApp.CommonSdk.Model.User;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DeviceMonitoring.Controllers
{


    public class EquipmentInfoController : BaseApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 1.接收温湿度数据
        /// </summary>

        [HttpPost]
        public IHttpActionResult SubmitTemAndHumData(DeviceEnvironmentDataInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.SubmitTemAndHumData(parameter);
            return InspurJson<RetDeviceEnvironmentDataInfo>(get);
        }
        // <summary>
        /// 2.从数据库中获取最新温湿度数据
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetTemAndHumData(GetDeviceEnvironmentDataInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.GetTemAndHumData(parameter);
            return InspurJson<RetEnvironmentInfo>(get);
        }

        /// <summary>
        /// 3.药品信息录入到数据库
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddMedicineInfo(MedicineInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.AddMedicineInfo(parameter);
            return InspurJson<RetMedicineInfo>(get);
        }

        /// <summary>
        /// 4.药品信息查询 
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetMedicineInfo(GetMedicineInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.GetMedicineInfo(parameter);
            return InspurJson <List<RetMedicineDetails>> (get);
        }
        /// <summary>
        /// 5.药品详情展示 搜索查询
        /// </summary>

        [HttpPost]
        public IHttpActionResult DisplayMedicineInfo(DisplayMedicineInfo parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.DisplayMedicineInfo(parameter);
            return InspurJson<List<RetDisplayMedicineInfo>>(get);
        }

        [HttpPost]
        public IHttpActionResult FindMedicineInfo(FindMedicineInfo parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.FindMedicineInfo(parameter);
            return InspurJson<List<RetFindMedicineInfo>>(get);
        }

        /// <summary>
        /// 6.药品详情展示 扫码查询
        /// </summary>

        [HttpPost]
        public IHttpActionResult ScanMedicineInfo(DisplayMedicineInfo parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.ScanMedicineInfo(parameter);
            return InspurJson<List<RetDisplayMedicineInfo>>(get);
        }
        /// <summary>
        /// 7.已有药品的库存存入，本部分实现对已有药品进行库存进行更改
        /// </summary>

        [HttpPost]
        public IHttpActionResult UpdateMedicineInfo(MedicineInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.UpdateMedicineInfo(parameter);
            return InspurJson<RetMedicineInfo>(get);
        }

        /// <summary>
        /// 8.药品取出
        /// </summary>

        [HttpPost]
        public IHttpActionResult DeleteMedicineInfo(MedicineInfoModel parameter)    //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.DeleteMedicineInfo(parameter);
            return InspurJson<RetMedicineInfo>(get);
        }
        /// <summary>
        /// 9.药柜药格删除
        /// </summary>

        [HttpPost]
        public IHttpActionResult DeleteMedicineCabintId(MedicineInfoModel parameter)    //这里写需要接收的参数)
        {

            DeviceInfoBLL data = new DeviceInfoBLL();
            var get = data.DeleteMedicineCabintId(parameter);
            return InspurJson<RetMedicineInfo>(get);
        }

        /// <summary>
        /// 9.设备绑定
        /// </summary>

        [HttpPost]
        public IHttpActionResult DeviceBuilding(DeviceBuildingModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.DeviceBuilding(parameter);
            return InspurJson<RetDeviceBuilding>(get);
        }

        /// <summary>
        /// 9.1查询IMEI下所有绑定的设备
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetDeviceBuilding(DeviceBuildingModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceBuilding(parameter);
            return InspurJson<List<RetDeviceBuilding>>(get);
        }

        /// <summary>
        /// 9.2切换药柜时选择的是药柜名称
        /// </summary>

        [HttpPost]
        public IHttpActionResult GetDeviceTableId(DeviceBuildingModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceTableId(parameter);
            return InspurJson<RetDeviceBuilding>(get);
        }
        /// <summary>
        /// 9.3删除设备
        /// </summary>
        [HttpPost]
        public IHttpActionResult DeleteDeviceInfo(DeviceBuildingModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.DeleteDevice(parameter);
            return InspurJson<RetDeviceBuilding>(get);
        }


        /// <summary>
        /// 10.获取温度报表数据
        /// </summary>

        [HttpPost]
        public RetGetTemChart GetTemChart(GetTemChartModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetTemChart(parameter);
            return get;
        }



        /// <summary>
        /// 11.获取湿度报表数据
        /// </summary>
        [HttpPost]
        public RetGetHumChart GetHumChart(GetTemChartModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetHumChart(parameter);   
            return get;
        }

        /// <summary>
        /// 12.获取历史开关门状态
        /// </summary>
        [HttpPost]
        public RetGetStatusChart GetStatusChart(GetTemChartModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetStatusChart(parameter);
            return get;
        }

        /// <summary>
        /// 13.制定报警策略
        /// </summary>

        [HttpPost]
        public IHttpActionResult AlarmStrategy(AlertPoliciesModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AlertPolicies(parameter);
            return InspurJson<RetAlertPolicies>(get);
        }

        /// <summary>
        /// 14.报警信息写入数据库
        /// </summary>

        [HttpPost]
        public IHttpActionResult AlarmInfo(AlertInfoModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AlarmInfo(parameter);
            return InspurJson<List<DeviceMonitoringDAL.D_AlarmStrategy>>(get);
        }

       
        

        /// <summary>
        /// 15.开关门报警信息写入数据库
        /// </summary>

        [HttpPost]
        public IHttpActionResult AlarmDoorInfo(AlertDoorModel parameter)                   //这里写需要接收的参数)
        {

            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AlarmDoorInfo(parameter);
            return InspurJson<List<DeviceMonitoringDAL.D_AlarmDoorPolicies>>(get);
        }
        // <summary>
        /// 18.开关门报警信息查询
        /// </summary>
     
        /// <summary>
        /// 获取设备模板列表信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceModelInfo(parameter);
            return InspurJson<List<RetDeviceModelInfo>>(get);
        }

        /// <summary>
        /// 新增设备模板
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = userApi.Data.UserId;
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AddDeviceModelInfo(parameter);
            return InspurJson<RetDeviceModelInfo>(get);
        }

        /// <summary>
        /// 根据ID获取设备模板信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetDeviceModelInfoByID(DeviceModelInfoModel parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceModelInfoByID(parameter);
            return InspurJson<RetDeviceModelInfo>(get);
        }
        /// <summary>
        /// 获取所有设备模板信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAllDeviceModelInfo()
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetAllDeviceModelInfo();
            return InspurJson<List<RetDeviceModelInfo>>(get);
        }
        /// <summary>
        /// 根据设备模板ID获取该模板的所有设备
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult GetDevicesByID(DeviceModelInfoModel parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDevicesByID(parameter);
            return InspurJson<List<RetDeviceInfo>>(get);
        }
        /// <summary>
        /// 更新设备模板
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult UpdateDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.UpdateDeviceModelInfo(parameter);
            return InspurJson<RetDeviceModelInfo>(get);
        }

        /// <summary>
        /// 删除设备模板
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult DeleteDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.DeleteDeviceModelInfo(parameter);
            return InspurJson<RetDeviceModelInfo>(get);
        }

        /// <summary>
        /// 查询设备模板编号是否存在
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult CheckDeviceInfoByModelLabel(DeviceModelInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.CheckDeviceInfoByModelLabel(parameter);
            return InspurJson<RetDeviceModelInfo>(get);
        }

        /// <summary>
        /// 查询设备模板属性标识符是否存在
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult CheckDeviceInfoByPropertyLabel(DeviceModelItemInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.CheckDeviceInfoByPropertyLabel(parameter);
            return InspurJson<RetDeviceModelItemInfo>(get);
        }

        /// <summary>
        /// 获取设备分组信息(非级联)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult GetDeviceGroupInfo(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceGroupInfo(parameter);
            
            return InspurJson<List<RetDeviceGroupInfo>>(get);
        }


        /// --------新加的----
        /// <summary>
        /// 根据ID返回设备分组信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// 
        
        [HttpPost]
        public IHttpActionResult GetDeviceGroupInfoById(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceGroupInfoById(parameter);
            return InspurJson<List<RetDeviceGroupInfo>>(get);
        }

        
        [HttpPost]
        public IHttpActionResult GetDeviceGroupCasInfo(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceGroupCasInfo(parameter);
            return InspurJson<List<RetGroupCascader>>(get);
        }

        /// -------------------新加的-----------------
        /// <summary>
        /// 获取设备分组信息(由一级得到所有目录的信息,由建筑物的信息得到所有楼和教室的信息)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult GetDeviceGroupByRoot(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDeviceGroupInfoByRoot(parameter);
            return InspurJson<List<RetGroupCascader>>(get);
        }

        /// -------------------新加的-----------------
        /// <summary>
        /// 获取设备一级分组信息(父结点为null的)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult GetRootGroup()
        {
            DeviceGroupModel parameter = new DeviceGroupModel();
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetRootGroup(parameter);
            return InspurJson<List<DeviceGroupModel>>(get);
        }


        /// <summary>
        /// 新增设备分组信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult AddDeviceGroupInfo(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = userApi.Data.UserId;
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AddDeviceGroupInfo(parameter);
            return InspurJson<RetDeviceGroupInfo>(get);
        }

        ///---新加--
        /// <summary>
        /// 为设备分组添加图片
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult AddDeviceGroupImage(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.CreateUserID = userApi.Data.UserId;
            parameter.OrgID = userApi.Data.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.AddDeviceGroupImage(parameter);
            return InspurJson<RetDeviceGroupInfo>(get);
        }

        /// <summary>
        /// 更新设备分组信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult UpdateDeviceGroupInfo(DeviceGroupModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.UpdateUserID = userApi.Data.UserId;
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.UpdateDeviceGroupInfo(parameter);
            return InspurJson<RetDeviceGroupInfo>(get);
        }

        /// <summary>
        /// 删除设备分组信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [HttpPost]
        public IHttpActionResult DeleteDeviceGroupInfo(DeviceGroupModel parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.DeleteDeviceGroupInfo(parameter);
            return InspurJson<RetDeviceGroupInfo>(get);
        }

        /// <summary>
        /// 查询设备标识符是否存在
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult CheckDeviceInfoByDeviceLabel(DeviceInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.CheckDeviceInfoByDeviceLabel(parameter);
            return InspurJson<RetDeviceInfo>(get);
        }

        /// <summary>
        /// 查询设备属性标识符是否存在
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult CheckDeviceTtemByPropertyLabel(DeviceItemInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.CheckDeviceTtemByPropertyLabel(parameter);
            return InspurJson<RetDeviceItemInfo>(get);
        }

        /// <summary>
        /// 获取设备列表(无属性)
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GetEquipmentList_NoItem(DeviceInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.UserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetEquipmentList(parameter, false);
            return InspurJson<List<RetDeviceInfo>>(get);
        }

        /// <summary>
        /// 获取设备列表(包含属性)
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult GetEquipmentList(DeviceInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.UserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetEquipmentList(parameter);
            return InspurJson<List<RetDeviceInfo>>(get);
        }

        /// <summary>
        /// 获取设备列表(包含属性)，只获取汉王的
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult GetEquipmentListHanwang(DeviceInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.UserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetEquipmentListHanwang(parameter);
            return InspurJson<List<RetDeviceInfo>>(get);
        }

        /// <summary>
        /// 获取设备属性列表
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult GetEquipmentItemsList(DeviceItemInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetEquipmentItemsList(parameter);
            return InspurJson<List<RetDeviceItemInfo>>(get);
        }

        /// <summary>
        /// 获取设备属性列表,获取汉王的属性
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEquipmentItemsListOfHanwang(DeviceItemInfoModel parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            parameter.DeviceID = 82;//二年级二班的汉王霾表的ID
            var get = device.GetEquipmentItemsList(parameter);
            return InspurJson<List<RetDeviceItemInfo>>(get);
        }


        /// <summary>
        /// 启用/禁用设备
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult EnabledDevice(DeviceInfoModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UpdateUserID = userApi.Data.UserId;
            // 是否需要禁用启用服务
            AlertPoliciesApi alertApi = new AlertPoliciesApi();
            EnableDeviceParmeter par = new EnableDeviceParmeter();
            par.IsEnable = model.Status == "1" ? "true" : "false";
            par.DeviceId = model.ID.ToString();
            alertApi.EnanbleDeviceService(par);
            // 修改设备状态
            DeviceInfoBLL device = new DeviceInfoBLL();
            var change = device.EnabledDevice(model);
            return InspurJson<RetDeviceInfo>(change);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteDevice(DeviceInfoModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var delete = device.DeleteDevice(model);
            return InspurJson<RetDeviceInfo>(delete);
        }

        /// <summary>
        /// 新增设备
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddDevice(DeviceInfoModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UserID = Convert.ToInt32(userApi.Data.UserId);
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var add = device.AddDevice(model);
            return InspurJson<RetDeviceInfo>(add);
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetDevice(DeviceInfoModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDevice(model);
            return InspurJson<RetDeviceInfo>(get);
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult UpdateDevice(DeviceInfoModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UpdateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var update = device.UpdateDevice(model);
            return InspurJson<RetDeviceInfo>(update);
        }

        /// <summary>
        /// 获取所有设备列表(内部调用)
        /// </summary>
        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetAllDeviceListInside(GetDeviceInfoParameter parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            return InspurJson(device.GetDeviceListInside(parameter), true);
        }

        /// <summary>
        /// 获取当前组织设备列表(内部调用)
        /// </summary>
        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetDeviceListInside(GetDeviceInfoParameter parameter)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfo(new GetUserInfoParameter() { TokenId = parameter.TokenID });
            parameter.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            return InspurJson(device.GetDeviceListInside(parameter), true);
        }

        /// <summary>
        /// 利用ID/DeviceLabel/OrgID/Phone获取设备信息(内部调用)
        /// </summary>
        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetDeviceInfoInside(GetDeviceInfoParameter parameter)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            DeviceInfoModel model = new DeviceInfoModel()
            {
                ID = null==parameter.ID?0:long.Parse(parameter.ID),
                DeviceLabel = parameter.DeviceLabel,
                OrgID = parameter.OrgID,
                Phone = parameter.Phone
            };
            return InspurJson(device.GetDevice(model), true);
        }

        /// <summary>
        /// 获取数据库连接配置列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetDataBaseList(DataConnectConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UserID = Convert.ToInt32(userApi.Data.UserId);
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDataBaseList(model);
            return InspurJson<List<RetDataConnectConfiguration>>(get);
        }

        /// <summary>
        /// 获取数据库连接配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetDataConnect(DataConnectConfigurationModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetDataConnect(model);
            return InspurJson<RetDataConnectConfiguration>(get);
        }

        /// <summary>
        /// 获取数据库连接配置(内部调用)
        /// </summary>
        [InnerCallFilter]
        [HttpPost]
        public IHttpActionResult GetDataConnectInner(GetIoTHubConnectDataParameter model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            DataConnectConfigurationModel par = new DataConnectConfigurationModel() {
                ID = long.Parse(model.ID),
                UserID = 0,
            };
            var get = device.GetDataConnect(par);
            return InspurJson<RetDataConnectConfiguration>(get,true);
        }

        /// <summary>
        /// 新增数据库连接配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddDataConnect(DataConnectConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UserID = Convert.ToInt32(userApi.Data.UserId);
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var add = device.AddDataConnect(model);
            return InspurJson<RetDataConnectConfiguration>(add);
        }

        /// <summary>
        /// 更新数据库连接配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult EditDataConnect(DataConnectConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UpdateUserID = userApi.Data.UserId;
            DeviceInfoBLL device = new DeviceInfoBLL();
            var update = device.EditDataConnect(model);
            return InspurJson<RetDataConnectConfiguration>(update);
        }

        /// <summary>
        /// 删除数据库连接配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteDataConnect(DataConnectConfigurationModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var delete = device.DeleteDataConnect(model);
            return InspurJson<RetDataConnectConfiguration>(delete);
        }

        /// <summary>
        /// 获取物接入配置列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetIoTHubDataList(IoTHubConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetIoTHubDataList(model);
            return InspurJson<List<RetIoTHubConfiguration>>(get);
        }

        /// <summary>
        /// 获取物接入配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetIoTHubData(IoTHubConfigurationModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetIoTHubData(model);
            return InspurJson<RetIoTHubConfiguration>(get);
        }

        /// <summary>
        /// 新增物接入配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult AddIoTHubData(IoTHubConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.CreateUserID = userApi.Data.UserId;
            model.OrgID = userApi.Data.OrgID.ToString();
            DeviceInfoBLL device = new DeviceInfoBLL();
            var add = device.AddIoTHubData(model);
            return InspurJson<RetIoTHubConfiguration>(add);
        }

        /// <summary>
        /// 更新物接入配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult EditIoTHubData(IoTHubConfigurationModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.UpdateUserID = userApi.Data.UserId;
            DeviceInfoBLL device = new DeviceInfoBLL();
            var update = device.EditIoTHubData(model);
            return InspurJson<RetIoTHubConfiguration>(update);
        }

        /// <summary>
        /// 删除物接入配置
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DeleteIoTHubData(IoTHubConfigurationModel model)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            var delete = device.DeleteIoTHubData(model);
            return InspurJson<RetIoTHubConfiguration>(delete);
        }

        /// <summary>
        /// 获取物接入配置(内部调用)
        /// </summary>
        [InnerCallFilter]
        [HttpPost]
        public IHttpActionResult GetIoTHubDataInner(GetIoTHubConnectDataParameter parm)
        {
            DeviceInfoBLL device = new DeviceInfoBLL();
            IoTHubConfigurationModel model = new IoTHubConfigurationModel();
            model.ID = long.Parse(parm.ID);
            var get = device.GetIoTHubData(model);
            return InspurJson<RetIoTHubConfiguration>(get, true);
        }

        /// <summary>
        /// 获取设备运行报表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEquipmentReportList(EquipmentReportModel model)
        {
            ReturnItem<List<RetEquipmentReport>> r = new ReturnItem<List<RetEquipmentReport>>();
            List<RetEquipmentReport> reportlist = new List<RetEquipmentReport>();
            DeviceInfoModel parameter = new DeviceInfoModel();
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            parameter.UserID = Convert.ToInt32(userApi.Data.UserId);
            parameter.OrgID = userApi.Data.OrgID.ToString();
            parameter.Name = model.Name;
            parameter.Status = "1"; // 启用的设备
            parameter.GroupIDList = model.GroupIDList;
            parameter.PageIndex = model.PageIndex;
            parameter.PageSize = model.PageSize;
            DeviceInfoBLL device = new DeviceInfoBLL();
            var get = device.GetEquipmentList(parameter);

            DeviceDataBLL data = new DeviceDataBLL();
            List<RetDeviceInfo> list = get.Data;
            foreach (var item in list)
            {
                RetEquipmentReport reportlistitem = new RetEquipmentReport();
                DeviceDataModel datamodel = new DeviceDataModel();
                datamodel.DeviceID = item.ID;
                datamodel.DeviceItemID = item.DeviceItems[0].ID;
                datamodel.StartTime = model.StartTime;
                datamodel.EndTime = model.EndTime;
                datamodel.StatisticalInterval = model.StatisticalInterval;
                datamodel.IntervalUnit = model.IntervalUnit;
                var getchartdata = data.GetEquipmentReportListData(datamodel);
                List<object> timelist = getTime(model, getchartdata.Data); // 获取运行时长、停运时长、开机率
                List<StateDataModel> datalist = getStateData(model, getchartdata.Data); // 获取设备运行状态
                reportlistitem.Name = item.Name;
                reportlistitem.DeviceInfo = item;
                reportlistitem.TotalRun = timelist[0].ToString();
                reportlistitem.TotalStop = timelist[1].ToString();
                reportlistitem.OpeningRate = Convert.ToDouble(timelist[2]);
                reportlistitem.StateDataList = datalist;
                reportlist.Add(reportlistitem);
            }
            r.Count = get.Count;
            r.Code = 0;
            r.Msg = "设备信息获取成功";
            r.Data = reportlist;
            return InspurJson<List<RetEquipmentReport>>(r);
        }

        private static List<object> getTime(EquipmentReportModel model, RetDeviceTableData data)
        {
            var allTime = model.EndTime - model.StartTime;
            double totalRun = 0.00;
            double totalStop = 0.00;
            double openingRate = 0.00;
            if (data != null)
            {
                if (data.Data.Count > 0)
                {
                    var count = data.DeviceTableList.Count;
                    List<TimeSpan?> timespan = new List<TimeSpan?>();
                    for (var i = 0; i < data.DeviceTableList.Count - 1; i++)
                    {
                        var time1 = data.DeviceTableList[i].Time;
                        var time2 = data.DeviceTableList[i + 1].Time;
                        var midTime = time1 - time2;
                        timespan.Add(midTime);
                    }
                    TimeSpan? time = timespan.Min(x => x);
                    totalRun = time.Value.TotalSeconds * count;
                    totalStop = allTime.Value.TotalSeconds - totalRun;
                    openingRate = Math.Round((totalRun / allTime.Value.TotalSeconds), 4);
                }
                else
                {
                    totalRun = 0.00;
                    totalStop = allTime.Value.TotalSeconds;
                    openingRate = 0.00;
                }
            }
            else
            {
                totalRun = 0.00;
                totalStop = allTime.Value.TotalSeconds;
                openingRate = 0.00;
            }
            string TotalRun = "";
            string TotalStop = "";
            if (model.IntervalUnit == "1")
            {
                TotalRun = totalRun.ToString() + "秒";
                TotalStop = totalStop.ToString() + "秒";
            }
            else if (model.IntervalUnit == "2")
            {
                TotalRun = Math.Round((totalRun / 60), 2).ToString() + "分";
                TotalStop = Math.Round((totalStop / 60), 2).ToString() + "分";
            }
            else if (model.IntervalUnit == "3")
            {
                TotalRun = Math.Round((totalRun / 3600), 2).ToString() + "小时";
                TotalStop = Math.Round((totalStop / 3600), 2).ToString() + "小时";
            }
            else if (model.IntervalUnit == "4")
            {
                TotalRun = Math.Round((totalRun / 86400), 2).ToString() + "天";
                TotalStop = Math.Round((totalStop / 86400), 2).ToString() + "天";
            }
            else
            {
                TotalRun = totalRun.ToString() + "秒";
                TotalStop = totalStop.ToString() + "秒";
            }
            List<object> list = new List<object>() { TotalRun, TotalStop, openingRate };
            return list;
        }

        private static List<StateDataModel> getStateData(EquipmentReportModel model, RetDeviceTableData data)
        {
            List<StateDataModel> list = new List<StateDataModel>();
            if (data != null)
            {
                if (data.Data.Count > 0)
                {
                    var count = data.DeviceTableList.Count;
                    List<TimeSpan?> timespan = new List<TimeSpan?>();
                    for (var i = 0; i < data.DeviceTableList.Count - 1; i++)
                    {
                        var time1 = data.DeviceTableList[i].Time;
                        var time2 = data.DeviceTableList[i + 1].Time;
                        var midTime = time1 - time2;
                        timespan.Add(midTime);
                    }
                    TimeSpan? time = timespan.Min(x => x);
                    List<StateDataModel> StateDataList = new List<StateDataModel>();
                    // 设备运行状态
                    DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    long timeStamp1 = (long)(model.StartTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                    long timeStamp2 = (long)(model.EndTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                    long IntervalTime = Convert.ToInt32(time.Value.TotalMilliseconds); // 间隔时间
                    for (int j = 0; j < data.Time.Count; j++)
                    {
                        long timeStamp3 = (long)(Convert.ToDateTime(data.Time[j]) - localTime).TotalMilliseconds; // 相差毫秒数
                        if (j == 0)
                        {
                            if (timeStamp3 == timeStamp1)
                            {
                                ColorModel color = new ColorModel() { color = "#00B14F" };
                                ItemStyleModel ItemStyle = new ItemStyleModel() { normal = color };
                                StateDataModel StateData = new StateDataModel()
                                {
                                    Name = "运行",
                                    value = new List<long>() { 0, timeStamp1, timeStamp1 + IntervalTime, IntervalTime },
                                    itemStyle = ItemStyle
                                };
                                StateDataList.Add(StateData);
                            }
                            else if (timeStamp3 > timeStamp1)
                            {
                                ColorModel color = new ColorModel() { color = "#ff0000" };
                                ItemStyleModel ItemStyle = new ItemStyleModel() { normal = color };
                                StateDataModel StateData = new StateDataModel()
                                {
                                    Name = "停机",
                                    value = new List<long>() { 0, timeStamp1, timeStamp3, timeStamp3 - timeStamp1 },
                                    itemStyle = ItemStyle
                                };
                                StateDataList.Add(StateData);
                                ColorModel color1 = new ColorModel() { color = "#00B14F" };
                                ItemStyleModel ItemStyle1 = new ItemStyleModel() { normal = color1 };
                                StateDataModel StateData1 = new StateDataModel()
                                {
                                    Name = "运行",
                                    value = new List<long>() { 0, timeStamp3, timeStamp3 + IntervalTime, IntervalTime },
                                    itemStyle = ItemStyle1
                                };
                                StateDataList.Add(StateData1);
                            }
                        }
                        else if (j > 0)
                        {
                            long timeStamp4 = (long)(Convert.ToDateTime(data.Time[j - 1]) - localTime).TotalMilliseconds; // 相差毫秒数
                            if (timeStamp4 + IntervalTime == timeStamp3)
                            {
                                ColorModel color = new ColorModel() { color = "#00B14F" };
                                ItemStyleModel ItemStyle = new ItemStyleModel() { normal = color };
                                StateDataModel StateData = new StateDataModel()
                                {
                                    Name = "运行",
                                    value = new List<long>() { 0, timeStamp3, timeStamp3 + IntervalTime, IntervalTime },
                                    itemStyle = ItemStyle
                                };
                                StateDataList.Add(StateData);
                            }
                            else if (timeStamp4 + IntervalTime < timeStamp3)
                            {
                                ColorModel color = new ColorModel() { color = "#ff0000" };
                                ItemStyleModel ItemStyle = new ItemStyleModel() { normal = color };
                                StateDataModel StateData = new StateDataModel()
                                {
                                    Name = "停机",
                                    value = new List<long>() { 0, timeStamp4 + IntervalTime, timeStamp3, timeStamp3 - timeStamp4 - IntervalTime },
                                    itemStyle = ItemStyle
                                };
                                StateDataList.Add(StateData);
                                ColorModel color1 = new ColorModel() { color = "#00B14F" };
                                ItemStyleModel ItemStyle1 = new ItemStyleModel() { normal = color1 };
                                StateDataModel StateData1 = new StateDataModel()
                                {
                                    Name = "运行",
                                    value = new List<long>() { 0, timeStamp3, timeStamp3 + IntervalTime, IntervalTime },
                                    itemStyle = ItemStyle1
                                };
                                StateDataList.Add(StateData1);
                            }
                            if (j == data.Time.Count - 1)
                            {
                                if (timeStamp3 + IntervalTime < timeStamp2)
                                {
                                    ColorModel color = new ColorModel() { color = "#ff0000" };
                                    ItemStyleModel ItemStyle = new ItemStyleModel() { normal = color };
                                    StateDataModel StateData = new StateDataModel()
                                    {
                                        Name = "停机",
                                        value = new List<long>() { 0, timeStamp3 + IntervalTime, timeStamp2, timeStamp2 - timeStamp3 - IntervalTime },
                                        itemStyle = ItemStyle
                                    };
                                    StateDataList.Add(StateData);
                                }
                            }
                        }
                    }
                    list = StateDataList;
                }
                else
                {
                    List<StateDataModel> StateDataList = new List<StateDataModel>();
                    // 设备运行状态
                    DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    long timeStamp1 = (long)(model.StartTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                    long timeStamp2 = (long)(model.EndTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                    ColorModel color = new ColorModel()
                    {
                        color = "#ff0000" // 停机
                    };
                    ItemStyleModel ItemStyle = new ItemStyleModel()
                    {
                        normal = color
                    };
                    StateDataModel StateData = new StateDataModel()
                    {
                        Name = "停机",
                        value = new List<long>() { 0, timeStamp1, timeStamp2, timeStamp2 - timeStamp1 },
                        itemStyle = ItemStyle
                    };
                    StateDataList.Add(StateData);
                    list = StateDataList;
                }
            }
            else
            {
                List<StateDataModel> StateDataList = new List<StateDataModel>();
                // 设备运行状态
                DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                long timeStamp1 = (long)(model.StartTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                long timeStamp2 = (long)(model.EndTime - localTime).Value.TotalMilliseconds; // 相差毫秒数
                ColorModel color = new ColorModel()
                {
                    color = "#ff0000" // 停机
                };
                ItemStyleModel ItemStyle = new ItemStyleModel()
                {
                    normal = color
                };
                StateDataModel StateData = new StateDataModel()
                {
                    Name = "停机",
                    value = new List<long>() { 0, timeStamp1, timeStamp2, timeStamp2 - timeStamp1 },
                    itemStyle = ItemStyle
                };
                StateDataList.Add(StateData);
                list = StateDataList;
            }
            return list;
        }
    }
}