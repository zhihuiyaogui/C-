using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDAL.Model.Return.User
{
    public class U_PhoneCode

    {
        public string mobile { get; set; }
        public string Phone { get; set; }

        public string Code { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }

       
    }
}
