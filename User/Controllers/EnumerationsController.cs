using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.Enumerations;
using UserBLL.Model.Return.Enumerations;

namespace User.Controllers
{
    public class EnumerationsController : BaseApiController
    {
        /// <summary>
        /// 获取下拉数据（一级菜单）
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = false, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetEnumerations(EnumerationsModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID;
            EnumerationsBLL user = new EnumerationsBLL();
            var get = user.GetEnumerations(model);
            return InspurJson<List<RetEnumerations>>(get);
        }

        /// <summary>
        /// 获取下拉数据（二级菜单）
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = false, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetSecondLevelEnumerations(EnumerationsModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.OrgID = userApi.Data.OrgID;
            EnumerationsBLL user = new EnumerationsBLL();
            var get = user.GetSecondLevelEnumerations(model);
            return InspurJson<List<RetEnumerations>>(get);
        }
    }
}