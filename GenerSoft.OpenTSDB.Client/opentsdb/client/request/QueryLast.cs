using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client.opentsdb.client.request
{
    class QueryLast
    {
        public bool resolveNames = true;

        public int backScan { get; set; }

        public List<SubQueryLast> queries = new List<SubQueryLast>();



        internal List<SubQueryLast> Queries
        {
            get
            {
                return queries;
            }

            set
            {
                queries = value;
            }
        }

        public void addSubQuery(SubQueryLast item) {
            
            this.queries.Add(item);
        }


        public string build() {
            if (null == this.queries)
            {
                throw new Exception(" must contain at least one subQuery.");
            }
            return JsonConvert.SerializeObject(this);
        }
    }
    class SubQueryLast {

        public string metric {
            get; set;
        }
        public Dictionary<string, string> tags
        {
            get;
            set;
        }

        public SubQueryLast(string metric, Dictionary<string, string> tags)
        {
            this.metric = metric;
            this.tags = tags;
        }
        


    }
   
}
