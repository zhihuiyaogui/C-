using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class SimpleHttpResponse
    {
        private int statusCode;
        private String content;

        public bool isSuccess()
        {
            return statusCode == 200 || statusCode == 204;
        }

        public int getStatusCode()
        {
            return statusCode;
        }

        public void setStatusCode(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public String getContent()
        {
            return content;
        }

        public void setContent(String content)
        {
            this.content = content;
        }

    }
}
