using Common;
using Common.Config;
using Common.WebApiHelper;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceData;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace GenerSoft.IndApp.CommonSdk
{
    public class DeviceMonitoringApi
    {
        /// <summary>
        /// 获取所有设备列表
        /// </summary>
        /// <returns></returns>
        public ReturnItem<List<RetDeviceInfo>> GetAllDeviceList(GetDeviceInfoParameter parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentInfo/GetAllDeviceListInside" };
            if (!string.IsNullOrEmpty(parameter.Status))
            {
                wparameter.Content.Add("Status", parameter.Status);
            }
            return new WebApiHelper().GetEntity<List<RetDeviceInfo>>(wparameter);
        }

        /// <summary>
        /// 获取当前组织设备列表
        /// </summary>
        /// <returns></returns>
        public ReturnItem<List<RetDeviceInfo>> GetDeviceList(GetDeviceInfoParameter parameter)
        {
            string tokenId = "";
            var getdic = JsonHelper.JsonToEntity<Hashtable>(HttpContext.Current.Request.Headers["tokenid"].ToBase64DecryptString());
            tokenId = getdic["tokenid"].ToString();
            parameter.TokenId = tokenId;
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentInfo/GetDeviceListInside" };
            parameter.SetPostParameter(wparameter);//填充请求参数
            if (!string.IsNullOrEmpty(parameter.Status))
            {
                wparameter.Content.Add("Status", parameter.Status);
            }
            return new WebApiHelper().GetEntity<List<RetDeviceInfo>>(wparameter);
        }

        /// <summary>
        ///利用ID获取设备信息
        /// </summary>
        /// <returns></returns>
        public ReturnItem<RetDeviceInfo> GetDeviceInfo(GetDeviceInfoParameter parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentInfo/GetDeviceInfoInside" };
            if (!string.IsNullOrEmpty(parameter.ID))
            {
                wparameter.Content.Add("ID", parameter.ID.ToString());// 传递设备ID
            }
            if (!string.IsNullOrEmpty(parameter.DeviceLabel))
            {
                wparameter.Content.Add("DeviceLabel", parameter.DeviceLabel);
            }
            if (!string.IsNullOrEmpty(parameter.Phone))
            {
                wparameter.Content.Add("Phone", parameter.Phone);
            }
            if (!string.IsNullOrEmpty(parameter.OrgID))
            {
                wparameter.Content.Add("OrgID", parameter.OrgID);
            }
            return new WebApiHelper().GetEntity<RetDeviceInfo>(wparameter);
        }

        /// <summary>
        /// 获取设备实时数据
        /// </summary>
        /// <returns></returns>
        public ReturnItem<List<RetDeviceCurrentData>> GetDeviceCurrentData(List<GetDeviceDataParameter> parameter)
        {
            var getdic = JsonHelper.JsonToEntity<Hashtable>(HttpContext.Current.Request.Headers["tokenid"].ToBase64DecryptString());
            string tokenId = getdic["tokenid"].ToString();
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentData/GetDeviceCurrentDataInside" };
            string DeviceInfo = JsonConvert.SerializeObject(parameter);
            wparameter.Content.Add("DeviceInfo", DeviceInfo);
            wparameter.Content.Add("TokenID", tokenId);
            return new WebApiHelper().GetEntity<List<RetDeviceCurrentData>>(wparameter);
            //报错的地方
        }

        /// <summary>
        /// 获取物接入连接
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<RetIoTHubConfiguration> GetIoTHubConnection(IoTHubConfigurationModel parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentInfo/GetIoTHubDataInner" };
            string connectInfo = JsonConvert.SerializeObject(parameter);
            wparameter.Content.Add("ID", parameter.ID.ToString());
            return new WebApiHelper().GetEntity<RetIoTHubConfiguration>(wparameter);
        }

        /// <summary>
        /// 根据数据连接ID获取连接信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<RetDataConnectConfiguration> GetDataConnect(IoTHubConfigurationModel parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.DeviceApiUrl + "Api/EquipmentInfo/GetDataConnectInner" };
            string connectInfo = JsonConvert.SerializeObject(parameter);
            wparameter.Content.Add("ID", parameter.ID.ToString());
            return new WebApiHelper().GetEntity<RetDataConnectConfiguration>(wparameter);
        }
    }
}
