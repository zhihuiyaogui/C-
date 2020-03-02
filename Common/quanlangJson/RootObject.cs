using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.quanlangJson
{
    public class Dp_value
    {
        public string ID { get; set; }
        public string fanspeed { get; set; }
        public string fresh { get; set; }
        public string heat { get; set; }
        public string power { get; set; }
        public string sleep { get; set; }
        public string auto { get; set; }
        public string childlock { get; set; }
        public string lcd { get; set; }
        public string tempin { get; set; }
        public string tempout { get; set; }
        public string co2 { get; set; }
        public string pm25 { get; set; }
        public string humidity { get; set; }
        public string sur1 { get; set; }
        public string total1 { get; set; }
        public string sur2 { get; set; }
        public string total2 { get; set; }
        public string sur3 { get; set; }
        public string total3 { get; set; }
    }

    public class RootObject
    {
        public Dp_value dp_value { get; set; }
    }
}
