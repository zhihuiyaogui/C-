using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.CommonSdk.Model.User
{
    /// <summary>
    /// Token元素相关字段
    /// </summary>
    public class TokenEntity
    {
        public System.Guid TokenId { get; set; }

        public System.DateTime? CreationDate { get; set; }

        public System.DateTime? DisableDate { get; set; }

        public string UserId { get; set; }

        public int? UserForm { get; set; }
        /// <summary>
        /// 加密Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 加密向量
        /// </summary>
        public string IV { get; set; }

        public bool IsPlatformAdmin { get; set; }
    }
}
