using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.User
{
    public class GetUserInfoParameter
    {
        public string TokenId { get; set; }

        public void SetPostParameter(WebApiPostParameter postParameter)
        {
            if (TokenId != null && TokenId != "")
            {
                postParameter.Content.Add("TokenID", TokenId.ToString());
            }
            else
            {
                throw new Exception("Need TokenId");
            }
        }
    }
}
