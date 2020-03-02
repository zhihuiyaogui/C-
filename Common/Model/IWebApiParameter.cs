using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public interface IWebApiParameter
    {
        /// <summary>
        /// 封装WebApePostParameter,Post的主体字典
        /// </summary>
        /// <param name="postParameter"></param>
        void SetPostParameter(WebApiPostParameter postParameter);
    }
}
