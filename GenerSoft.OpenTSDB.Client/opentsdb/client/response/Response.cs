using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class Response {
        private int statusCode;
        private ErrorDetail errorDetail;

        public Response() {
        }

        public bool isSuccess() {
            return statusCode == 200 || statusCode == 204;
        }

        public Response(int statusCode) {
            this.statusCode = statusCode;
        }

        public int getStatusCode() {
            return statusCode;
        }

        public void setStatusCode(int statusCode) {
            this.statusCode = statusCode;
        }

        public ErrorDetail getErrorDetail() {
            return errorDetail;
        }

        public void setErrorDetail(ErrorDetail errorDetail) {
            this.errorDetail = errorDetail;
        }

        public override string ToString()
        {
            return "Response [statusCode=" + statusCode + ", errorDetail="
                    + errorDetail + "]";
        }
    }
}