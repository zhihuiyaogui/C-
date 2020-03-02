using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class Filter
    {
        private string type;
        private string tagk;
        private string filter;
        private bool groupBy = false;

        public string getType()
        {
            return type;
        }

        public void setType(string type)
        {
            this.type = type;
        }

        public string getTagk()
        {
            return tagk;
        }

        public void setTagk(string tagk)
        {
            this.tagk = tagk;
        }

        public string getFilter()
        {
            return filter;
        }

        public void setFilter(string filter)
        {
            this.filter = filter;
        }

        public bool getGroupBy()
        {
            return groupBy;
        }

        public void setGroupBy(bool groupBy)
        {
            this.groupBy = groupBy;
        }
    }
}