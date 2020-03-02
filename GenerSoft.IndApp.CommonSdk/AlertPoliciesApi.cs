using Common;
using Common.Config;
using Common.WebApiHelper;
using GenerSoft.IndApp.CommonSdk.Model.Alert;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GenerSoft.IndApp.CommonSdk
{
    public class AlertPoliciesApi
    {

        public ReturnItem<string> EnanbleDeviceService(EnableDeviceParmeter parameter)
        {
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.AlertApiUrl + "Api/AlertPolicies/EnableDeviceService" };
            wparameter.Content.Add("IsEnable", parameter.IsEnable);
            wparameter.Content.Add("DeviceId", parameter.DeviceId);
            return new WebApiHelper().GetEntity<string>(wparameter);
        }

        /// <summary>
        /// 获取报警策略个数
        /// </summary>
        /// <returns></returns>
        public ReturnItem<string> GetAlertPoliciesNum()
        {
            string tokenId = "";
            var getdic = JsonHelper.JsonToEntity<Hashtable>(HttpContext.Current.Request.Headers["tokenid"].ToBase64DecryptString());
            tokenId = getdic["tokenid"].ToString();
            WebApiPostParameter wparameter = new WebApiPostParameter() { Url = CustomConfigParam.AlertApiUrl + "Api/AlertPolicies/GetAlertPoliciesNum" };
            wparameter.Content.Add("TokenId", tokenId.ToString());
            return new WebApiHelper().GetEntity<string>(wparameter);
        }
    }
}
