using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.User
{
    public class GetTokenEntityParameter : IWebApiParameter
    {
        public Guid TokenId { get; set; }

        public void SetPostParameter(WebApiPostParameter postParameter)
        {
            if (TokenId != Guid.Empty)
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
