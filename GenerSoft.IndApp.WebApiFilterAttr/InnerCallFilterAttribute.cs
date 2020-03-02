using Common;
using Common.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GenerSoft.IndApp.WebApiFilterAttr
{
    public class InnerCallFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (filterContext.ActionArguments.Keys.ToList().Count > 0)
            {
                string key = filterContext.ActionArguments.Keys.ToList()[0];
                Type type = filterContext.ActionArguments[key].GetType();
                var get1 = filterContext.ActionArguments[key];

                PropertyInfo property = type.GetProperty("Tid");
                if (property == null)
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new ReturnItem<object>() { Code = -1, Msg = "接口不支持ApiToken验证，请联系接口提供方" });
                    return;
                }
                object o = property.GetValue(get1, null);

                if (o == null || o.ToString()==""|| o.ToString()!= CustomConfigParam.WebApiToken)
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new ReturnItem<object>() { Code = -1, Msg = "ApiToken不正确，无法访问接口" });
                    return;
                }
            }
            else
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new ReturnItem<object>() { Code = -1, Msg = "接口不支持ApiToken验证，请联系接口提供方" });
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
