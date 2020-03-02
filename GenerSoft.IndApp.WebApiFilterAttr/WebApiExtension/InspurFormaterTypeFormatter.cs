using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.WebApiFilterAttr.WebApiExtension
{
    public class InspurFormaterTypeFormatter : MediaTypeFormatter
    {
        
        public InspurFormaterTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/inspurformater"));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override async Task WriteToStreamAsync(Type type, object value,
            Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            using (var sw = new StreamWriter(writeStream))
            {
                await sw.WriteAsync(value.ToString());
            }
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream,
            HttpContent content, IFormatterLogger formatterLogger)
        {
            return null;
            //using (var sr = new StreamReader(readStream))
            //{
            //    return await sr.ReadToEndAsync();
            //}
        }
    }
}
