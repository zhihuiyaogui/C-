using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.quanlangJson
{
    public class judgeIsDeviceInfoSame
    {
        public static bool isSame(RootObject pre, RootObject after)
        {
            if (pre.dp_value.ID != after.dp_value.ID || pre.dp_value.fanspeed != after.dp_value.fanspeed || pre.dp_value.fresh != after.dp_value.fresh)
            {
                return false;
            }
            if (pre.dp_value.heat != after.dp_value.heat || pre.dp_value.power != after.dp_value.power || pre.dp_value.sleep != after.dp_value.sleep)
            {
                return false;
            }
            if (pre.dp_value.auto != after.dp_value.auto || pre.dp_value.childlock != after.dp_value.childlock || pre.dp_value.lcd != after.dp_value.lcd)
            {
                return false;
            }
            return true;
        }
    }
}
