using GenerSoft.IndApp.WebApiFilterAttr.WebApiExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DeviceMonitoring
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.Formatters.Add(new PlainTextTypeFormatter());
            config.Formatters.Add(new InspurFormaterTypeFormatter());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
