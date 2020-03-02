using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class ErrorDetail {
        private List<ErrorDetailEntity> errors = new List<ErrorDetailEntity>();

        public int success;
        public int failed;
        public ErrorDetail()
        {

        }
        public ErrorDetail(List<ErrorDetailEntity> errors) {
            this.errors = errors;
        }

        public ErrorDetail(int success, int failed) {
            this.success = success;
            this.failed = failed;
        }

        public ErrorDetail(int success, int failed,
                List<ErrorDetailEntity> errors) {
            this.success = success;
            this.failed = failed;

            this.errors = errors;
        }

        public ErrorDetail(ErrorDetailEntity error) {
            errors.Add(error);
        }

        public List<ErrorDetailEntity> getErrors() {
            return (errors);
        }

        public int getSuccess() {
            return success;
        }

        public void setSuccess(int success) {
            this.success = success;
        }

        public int getFailed() {
            return failed;
        }

        public void setFailed(int failed) {
            this.failed = failed;
        }

        public class ErrorDetailEntity {
            private DataPoint datapoint;
            private string error;

            public DataPoint getDatapoint() {
                return datapoint;
            }

            public void setDatapoint(DataPoint datapoint) {
                this.datapoint = datapoint;
            }

            public string getError() {
                return error;
            }

            public void setError(string error) {
                this.error = error;
            }

            public override string ToString() {
                return "ErrorDetailEntity [datapoint=" + datapoint + ", error="
                        + error + "]";
            }

        }

        public class DataPoint {
            private string metric;
            private long timestamp;

            private Object value;

            private IDictionary<string, string> tags = new Dictionary<string, string>();

            public string getMetric() {
                return metric;
            }

            public void setMetric(string metric) {
                this.metric = metric;
            }

            public long getTimestamp() {
                return timestamp;
            }

            public void setTimestamp(long timestamp) {
                this.timestamp = timestamp;
            }

            public Object getValue() {
                return value;
            }

            public void setValue(Object value) {
                this.value = value;
            }

            public IDictionary<string, string> getTags() {
                return tags;
            }

            public void setTags(IDictionary<string, string> tags) {
                this.tags = tags;
            }

        }

        public override string ToString() {
            return "ErrorDetail [" + "success=" + success + ", failed=" + failed
                    + ", errors=" + errors + "]";
        }
    }
}