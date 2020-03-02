using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    /**
     * Builder used to create the JSON to push metrics to KairosDB.
     */
    public class MetricBuilder {
        private List<Metric> metrics = new List<Metric>();

	private MetricBuilder() {
        }

        /**
         * Returns a new metric builder.
         *
         * @return metric builder
         */
        public static MetricBuilder getInstance() {
            return new MetricBuilder();
        }

        /**
         * Adds a metric to the builder.
         *
         * @param metricName
         *            metric name
         * @return the new metric
         */
        public Metric addMetric(string metricName) {
            Metric metric = new Metric(metricName);
            metrics.Add(metric);
            return metric;
        }

        /**
         * Returns a list of metrics added to the builder.
         *
         * @return list of metrics
         */
        public List<Metric> getMetrics() {
            return metrics;
        }

        /**
         * Returns the JSON string built by the builder. This is the JSON that can
         * be used by the client add metrics.
         *
         * @return JSON
         * @throws IOException
         *             if metrics cannot be converted to JSON
         */
        public string build() {
		foreach (Metric metric in metrics) {
                // verify that there is at least one tag for each metric
                if (metric.getTags().Count() <= 0)
                {
                    throw new Exception("there is at least one tag for each metric");
                }
            }
		return JsonConvert.SerializeObject(metrics);
        }
    }
}