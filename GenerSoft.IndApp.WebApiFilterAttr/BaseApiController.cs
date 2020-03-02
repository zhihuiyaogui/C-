using Common;
using Common.Config;
using GenerSoft.IndApp.CommonSdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace GenerSoft.IndApp.WebApiFilterAttr
{
    public class BaseApiController: ApiController
    {        
        public IHttpActionResult InspurJson<T>(ReturnItem<T> data)
        {
           /// if (CustomConfigParam.IsEncrypt)
          ///  {
                //加密输出
             ///   return new EncryptJsonResult(data);
         ///}
          ///  else
           /// {
                return base.Json<ReturnItem<T>>(data); 
           /// }
        }
        public IHttpActionResult InspurJson<T>(ReturnItem<T> data, bool disableEncrypt)
        {
          ///  if (disableEncrypt)
          ///  {
          ///      return base.Json<ReturnItem<T>>(data);
          ///  }
          ///  else
          ///  {
          ///      if (CustomConfigParam.IsEncrypt)
           ///     {
                    //加密输出
            ///        return new EncryptJsonResult(data);
             ///   }
              ///  else
             ///   {
                    return base.Json<ReturnItem<T>>(data);
                }
            }

        }
    

