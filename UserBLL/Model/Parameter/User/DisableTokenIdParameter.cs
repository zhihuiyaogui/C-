using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.User
{
    public class DisableTokenIdParameter
    {
        public string UserId { get; set; }
        public string TokenId { get; set; }
        public Guid MissionId { get; set; }
    }
}
