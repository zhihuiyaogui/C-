using Common;
using Common.Config;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DeviceMonitoring
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            log4net.Config.XmlConfigurator.Configure();
            DbInterception.Add(new CustomEFInterceptor());
        }

        public override void Init()
        {
            base.Init();
            //this.Error += WebApiApplication_Error;
            this.BeginRequest += WebApiApplication_BeginRequest;

        }

        void WebApiApplication_BeginRequest(object sender, EventArgs e)
        {
            log.Info("request info: "+new WebRequestInfo().ToString());
        }
    }
}
