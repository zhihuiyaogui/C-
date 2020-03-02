using Common.Config;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GenerSoft.IndApp.Upload.Controllers
{
    public class UploadController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Upload()
        {
            // Check whether the POST operation is MultiPart?  
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string fileSaveLocation = CustomConfigParam.UploadPath;
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string path = "";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                path = fileSaveLocation + strDate + "/";
            }
            else
            {
                path = fileSaveLocation + strDate + "\\";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form  
            // data will be loaded.  
            //string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(path);
            List<FileInfoModel> files = new List<FileInfoModel>();

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    string FileName = Path.GetFileName(file.LocalFileName);
                    FileInfoModel fileInfoModel = new FileInfoModel();
                    fileInfoModel.Name = FileName.Substring(FileName.LastIndexOf("\\") + 1, (FileName.LastIndexOf(".") - FileName.LastIndexOf("\\") - 1)); //文件名
                    fileInfoModel.Ext = FileName.Substring(FileName.LastIndexOf(".") + 1, (FileName.Length - FileName.LastIndexOf(".") - 1));//扩展名
                    fileInfoModel.MimeType = MimeMapping.GetMimeMapping(FileName);
                    fileInfoModel.Path = "\\" + strDate + "\\" + FileName;
                    fileInfoModel.Size = "";
                    fileInfoModel.Type = file.Headers.ContentType.MediaType.ToString();

                    files.Add(fileInfoModel);
                }

                // Send OK Response along with saved file names to the client.  
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        // We implement MultipartFormDataStreamProvider to override the filename of File which  
        // will be stored on server, or else the default name will be of the format like Body-  
        // Part_{GUID}. In the following implementation we simply get the FileName from   
        // ContentDisposition Header of the Request Body.  
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
}
