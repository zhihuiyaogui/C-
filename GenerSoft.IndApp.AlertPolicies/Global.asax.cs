using AlertPoliciesBLL;
using Common;
using Common.Config;
using GenerSoft.IndApp.AlertPoliciesBLL;
using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Web.Http;

namespace GenerSoft.IndApp.AlertPolicies
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            log4net.Config.XmlConfigurator.Configure();
            DbInterception.Add(new CustomEFInterceptor());
            if (CustomConfigParam.EnableMqtt)
            {
                AlertServiceBLL.AlertServiceStart();
            }
            ControllDeviceBLL.initSocket();
            storeAirData2Opentsdb.getDataServiceStart();
            log.Info("AlertPolicies Application Start End .");



        }

        public override void Init()
        {
            base.Init();
            this.BeginRequest += WebApiApplication_BeginRequest;
            //this.EndRequest += WebApiApplication_EndRequest;

        }

        void WebApiApplication_BeginRequest(object sender, EventArgs e)
        {
            log.Info("request info: " + new WebRequestInfo().ToString());
        }
    }
}
