using Common;
using Common.quanlangJson;
using GenerSoft.IndApp.AlertPoliciesBLL;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;



namespace GenerSoft.IndApp.AlertPolicies.Controllers
{
    public class ControllDeviceController : BaseApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
