using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.OpenTSDB.Client
{
    public class DateTimeUtil
    {
        public static DateTime parse(string date, string fm)
        {
            DateTime res ;
            try
            {
                res = DateTime.ParseExact(date, fm, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                throw new Exception("解析时间格式失败");
            }

            return res;
        }

    }
}
