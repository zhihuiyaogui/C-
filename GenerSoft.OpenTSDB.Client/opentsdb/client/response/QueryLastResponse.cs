using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client.opentsdb.client.response
{
    public class QueryLastResponse
    {
        public string timestamp { get; set; }

		public string value { get; set; }

		public string tsuid { get; set; }
    }
}
