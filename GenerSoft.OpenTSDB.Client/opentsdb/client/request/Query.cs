using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class Query
    {
        public long start;
        public long end;
        public List<SubQueries> queries = new List<SubQueries>();
        public bool noAnnotations = false;
        public bool globalAnnotations = false;
        public bool msResolution = false;
        public bool showTSUIDs = false;
        public bool showSummary = false;
        public bool showQuery = false;
        public bool delete = false;

        public Query()
        {
        }

        public Query(long start)
        {
            this.start = start;
        }

        public Query addSubQuery(SubQueries subQueries)
        {
            this.queries.Add(subQueries);
            return this;
        }

        public Query addStart(long start)
        {
            this.start = start;
            return this;
        }

        public Query addEnd(long end)
        {
            this.end = end;
            return this;
        }

        public long getStart()
        {
            return start;
        }

        public void setStart(long start)
        {
            this.start = start;
        }

        public long getEnd()
        {
            return end;
        }

        public void setEnd(long end)
        {
            this.end = end;
        }

        public List<SubQueries> getQueries()
        {
            return queries;
        }

        public void setQueries(List<SubQueries> queries)
        {
            this.queries = queries;
        }

        public bool getNoAnnotations()
        {
            return noAnnotations;
        }

        public void setNoAnnotations(bool noAnnotations)
        {
            this.noAnnotations = noAnnotations;
        }

        public bool getGlobalAnnotations()
        {
            return globalAnnotations;
        }

        public void setGlobalAnnotations(bool globalAnnotations)
        {
            this.globalAnnotations = globalAnnotations;
        }

        public bool getMsResolution()
        {
            return msResolution;
        }

        public void setMsResolution(bool msResolution)
        {
            this.msResolution = msResolution;
        }

        public bool getShowTSUIDs()
        {
            return showTSUIDs;
        }

        public void setShowTSUIDs(bool showTSUIDs)
        {
            this.showTSUIDs = showTSUIDs;
        }

        public bool getShowSummary()
        {
            return showSummary;
        }

        public void setShowSummary(bool showSummary)
        {
            this.showSummary = showSummary;
        }

        public bool getShowQuery()
        {
            return showQuery;
        }

        public void setShowQuery(bool showQuery)
        {
            this.showQuery = showQuery;
        }

        public bool getDelete()
        {
            return delete;
        }

        public void setDelete(bool delete)
        {
            this.delete = delete;
        }
    }
}