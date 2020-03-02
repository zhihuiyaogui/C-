using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class QueryBuilder
    {
        private Query query = new Query();

        private QueryBuilder()
        {
        }

        public static QueryBuilder getInstance()
        {
            return new QueryBuilder();
        }

        public Query getQuery()
        {
            return this.query;
        }

        public string build()
        {
            // verify that there is at least one tag for each metric
            if (!(query.getStart() > 0))
            {
                throw new Exception(" must contain start.");
            }
            if (query.getQueries() == null)
            {
                throw new Exception(" must contain at least one subQuery.");
            }
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(query,jsetting);
        }
    }
}