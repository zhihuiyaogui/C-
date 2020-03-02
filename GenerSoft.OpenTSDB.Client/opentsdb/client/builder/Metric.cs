using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{

    /**
     * A metric contains measurements or data points. Each data point has a time
     * stamp of when the measurement occurred and a value that is either a long or
     * double and optionally contains tags. Tags are labels that can be added to
     * better identify the metric. For example, if the measurement was done on
     * server1 then you might add a tag named "host" with a value of "server1". Note
     * that a metric must have at least one tag.
     */
    public class Metric {

        public string metric;

        public long timestamp;

        public Object value;

        public Dictionary<string, string> tags = new Dictionary<string, string>();

        public Metric(string name) {
            this.metric = Preconditions.checkNotNullOrEmpty(name);
        }

        /**
         * Adds a tag to the data point.
         *
         * @param name
         *            tag identifier
         * @param value
         *            tag value
         * @return the metric the tag was added to
         */
        public Metric addTag(string name, string value) {
            Preconditions.checkNotNullOrEmpty(name);
            Preconditions.checkNotNullOrEmpty(value);
            tags.Add(name, value);

            return this;
        }

        /**
         * Adds tags to the data point.
         * 
         * @param tags
         *            map of tags
         * @return the metric the tags were added to
         */
        public Metric addTags(IDictionary<string, string> tags) {

            foreach (var entry in tags)
            {
                this.tags.Add(entry.Key, entry.Value);
            }
            return this;
        }

        /**
         * set the data point for the metric.
         *
         * @param timestamp
         *            when the measurement occurred
         * @param value
         *            the measurement value
         * @return the metric
         */
        protected Metric innerAddDataPoint(long timestamp, Object value) {
            if (timestamp > 0)
                this.timestamp = timestamp;
            if (value != null)
                this.value = value;

            return this;
        }

        /**
         * Adds the data point to the metric with a timestamp of now.
         *
         * @param value
         *            the measurement value
         * @return the metric
         */
        public Metric setDataPoint(long value) {
            DateTime dt = DateTime.Now;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);
            return innerAddDataPoint(Convert.ToInt64((dt - start).TotalSeconds), value);
        }

        public Metric setDataPoint(long timestamp, long value) {
            return innerAddDataPoint(timestamp, value);
        }

        /**
         * Adds the data point to the metric.
         *
         * @param timestamp
         *            when the measurement occurred
         * @param value
         *            the measurement value
         * @return the metric
         */
        public Metric setDataPoint(long timestamp, double value) {
            return innerAddDataPoint(timestamp, value);
        }

        /**
         * Adds the data point to the metric with a timestamp of now.
         *
         * @param value
         *            the measurement value
         * @return the metric
         */
        public Metric setDataPoint(double value) {
            DateTime dt = DateTime.Now;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);
            return innerAddDataPoint(Convert.ToInt64((dt - start).TotalSeconds), value);
        }

        /**
         * Adds the data point to the metric with a timestamp of now.
         *
         * @param value
         *            the measurement value
         * @return the metric
         */
        public Metric setDataPoint(string value)
        {
            DateTime dt = DateTime.Now;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);
            return innerAddDataPoint(Convert.ToInt64((dt - start).TotalSeconds), value);
        }

        public Metric setDataPoint(long timestamp, string value)
        {
            return innerAddDataPoint(timestamp, value);
        }

        /**
         * Adds the data point to the metric with a timestamp of now.
         *
         * @param value
         *            the measurement value
         * @return the metric
         */
        public Metric setDataPoint(object value)
        {
            DateTime dt = DateTime.Now;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);
            return innerAddDataPoint(Convert.ToInt64((dt - start).TotalSeconds), value);
        }

        public Metric setDataPoint(long timestamp, object value)
        {
            return innerAddDataPoint(timestamp, value);
        }

        /**
         * Time when the data point was measured.
         *
         * @return time when the data point was measured
         */
        public long getTimestamp() {
            return timestamp;
        }

        public Object getValue() {
            return value;
        }

        public string stringValue() {
		return value.ToString();
        }

        public long longValue() {
		try {
                return Convert.ToInt64(value);
            } catch (Exception e) {
                throw new DataFormatException("Value is not a long");
            }
        }

        public double doubleValue() {
		try {
                return Convert.ToDouble(value);
            } catch (Exception e) {
                throw new DataFormatException("Value is not a double");
            }
        }

        public bool isDoubleValue() {
            return value.GetType() == typeof(double);
        }

        public bool isIntegerValue() {
            return value.GetType() == typeof(int);
        }

        /**
         * Returns the metric name.
         *
         * @return metric name
         */
        public string getName() {
            return metric;
        }

        /**
         * Returns the tags associated with the data point.
         *
         * @return tag for the data point
         */
        public IDictionary<string, string> getTags() {
            return tags;
        }

    }
}