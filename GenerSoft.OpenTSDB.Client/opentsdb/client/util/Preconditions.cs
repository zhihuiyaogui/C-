using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{

    public class Preconditions {
        public static string checkNotNullOrEmpty(string reference) {
            if(reference == null )
            {
                throw new Exception("参数不能为null");
            }
            if (reference=="") {
                throw new Exception("参数不能为空");
            }
            return reference;
        }

        public static string checkNotNullOrEmpty(string reference,
                 string errorMessageTemplate,
                 Object errorMessageArgs) {
            if (reference == null)
            {
                throw new Exception("参数不能为null");
            }
            if (reference == "")
            {
                throw new Exception("参数不能为空");
            }
            return reference;
        }

        /**
         * Copied from Google's Precondition class because it is package protected.
         *
         * Substitutes each {@code %s} in {@code template} with an argument. These
         * are matched by position - the first {@code %s} gets {@code args[0]}, etc.
         * If there are more arguments than placeholders, the unmatched arguments
         * will be appended to the end of the formatted message in square braces.
         *
         * @param template
         *            a non-null string containing 0 or more {@code %s}
         *            placeholders.
         * @param args
         *            the arguments to be substituted into the message template.
         *            Arguments are converted to strings using
         *            {@link string#valueOf(Object)}. Arguments can be null.
         */
        
    static string format(string template, Object args) {
            // start substituting the arguments into the '%s' placeholders
            /*StringBuilder builder = new StringBuilder(template.Length + 16
                    * args.length);
            int templateStart = 0;
            int i = 0;
            while (i < args.length) {
                int placeholderStart = template.indexOf("%s", templateStart);
                if (placeholderStart == -1) {
                    break;
                }
                builder.append(template.substring(templateStart, placeholderStart));
                builder.append(args[i++]);
                templateStart = placeholderStart + 2;
            }
            builder.append(template.substring(templateStart));

            // if we run out of placeholders, append the extra args in square braces
            if (i < args.length) {
                builder.append(" [");
                builder.append(args[i++]);
                while (i < args.length) {
                    builder.append(", ");
                    builder.append(args[i++]);
                }
                builder.append(']');
            }

            return builder.tostring();
            */
            return template;
        }

    }
}