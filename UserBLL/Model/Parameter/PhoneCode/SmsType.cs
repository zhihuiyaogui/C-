using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.Model.Parameter.PhoneCode
{
    public enum SmsType
    {
        [Description("验证码注册")]
        IdentifyingCode_Reg = 1,
        [Description("验证码重置密码")]
        IdentifyingCode_ResetPwd = 2,
        [Description("通知")]
        Notice = 3
    }
}
