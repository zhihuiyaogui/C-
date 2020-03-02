using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class JsonHelper
    {
        /// <summary>
        /// 反序列化到指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T JsonToEntity<T>(this string Json)
        {
            if (string.IsNullOrWhiteSpace(Json)) { return default(T); }
            else
            {
                return JsonConvert.DeserializeObject<T>(Json);
            }
        }
        /// <summary>
        /// 序列化对象到Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                //setting.Converters.Add(new CustomJsonConverter() { PropertyNullValueReplaceValue = "" });
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return setting;
            });
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static object ToJson(this string Json)
        {
            return JsonConvert.DeserializeObject(Json);
        }
    }
}
