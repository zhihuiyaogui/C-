using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{

    public abstract class HttpClient : Client {

        public abstract Response pushMetrics(MetricBuilder builder,
                                ExpectResponse exceptResponse);

        public abstract SimpleHttpResponse pushQueries(QueryBuilder builder,
                                          ExpectResponse exceptResponse);

        public abstract SimpleHttpResponse pushLastQueries(string content,
                                          ExpectResponse exceptResponse);
    }
}