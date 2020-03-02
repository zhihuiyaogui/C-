using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class SubQueries {
        public string aggregator;
        public string metric;
        public bool rate = false;
        public IDictionary<string, string> rateOptions;
        public string downsample;
        public IDictionary<string, string> tags = new Dictionary<string, string>();
        public List<Filter> filters = null;

        public SubQueries addAggregator(string aggregator) {
            this.aggregator = aggregator;
            return this;
        }

        public SubQueries addMetric(string metric) {
            this.metric = metric;
            return this;
        }

        public SubQueries addDownsample(string downsample) {
            this.downsample = downsample;
            return this;
        }

        /**
         * Tags are converted to filters in 2.2
         */
        
    public SubQueries addTag(IDictionary<string, string> tag) {
            foreach (var entry in tag)
            {
                this.tags.Add(entry.Key, entry.Value);
            }
            return this;
        }

        /**
         * Tags are converted to filters in 2.2
         */
        
    public SubQueries addTag(string tag, string value) {
            this.tags.Add(tag, value);
            return this;
        }

        public SubQueries addFilter(Filter filter) {
            if (null==this.filters)
            {
                filters = new List<Filter>();
            }
            this.filters.Add(filter);
            return this;
        }

        public string getAggregator() {
            return aggregator;
        }

        public void setAggregator(string aggregator) {
            this.aggregator = aggregator;
        }

        public string getMetric() {
            return metric;
        }

        public void setMetric(string metric) {
            this.metric = metric;
        }

        public bool getRate() {
            return rate;
        }

        public void setRate(bool rate) {
            this.rate = rate;
        }

        public IDictionary<string, string> getRateOptions() {
            return rateOptions;
        }

        public void setRateOptions(IDictionary<string, string> rateOptions) {
            this.rateOptions = rateOptions;
        }

        public string getDownsample() {
            return downsample;
        }

        public void setDownsample(string downsample) {
            this.downsample = downsample;
        }

        public IDictionary<string, string> getTags() {
            return tags;
        }

        /**
         * Tags are converted to filters in 2.2
         */
        public void setTags(IDictionary<string, string> tags) {
            this.tags = tags;
        }

        public List<Filter> getFilters() {
            return filters;
        }

        public void setFilters(List<Filter> filters) {
            this.filters = filters;
        }
    }
}