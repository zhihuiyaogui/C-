using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.Message
{
    public class GetMessageInfoParameter
    {
        public string TokenID { get; set; }
        public string Tid { get; set; }
        public string Type { get; set; }
        public string Tittle { get; set; }
        public string Text { get; set; }
        public string OrgID { get; set; }
    }
}
