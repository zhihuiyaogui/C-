using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.PhoneCode;

namespace UserBLL.SMS
{
    public class ShortMessageBLL
    {
        private static string SmsIsDebug = "";

        private static Dictionary<int, string> Msgs = null;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 相同IP每天可以发送的短信数量
        /// </summary>
        private static string IpSendNumber = "";

        private static string PhoneSendNumber = "";


        public static string GetSmsIsDebug()
        {
            if (SmsIsDebug == "")
            {
                SmsIsDebug = ConfigurationManager.AppSettings["SmsIsDebug"];
            }
            return SmsIsDebug;
        }

        public static string GetIpSendNumber()
        {
            if (IpSendNumber == "")
            {
                IpSendNumber = ConfigurationManager.AppSettings["IpSendNumber"];
            }
            return IpSendNumber;
        }

        public static string GetPhoneSendNumber()
        {
            if (PhoneSendNumber == "")
            {
                PhoneSendNumber = ConfigurationManager.AppSettings["PhoneSendNumber"];
            }
            return PhoneSendNumber;
        }

        public string GetMsgContent(int smstype)
        {
            if (Msgs == null)
            {
                Msgs = new Dictionary<int, string>();
            }

            if (Msgs.ContainsKey(smstype))
            {
                return Msgs[smstype];
            }

            if ((int)SmsType.IdentifyingCode_Reg == smstype)
            {
                string msgstr = ConfigurationManager.AppSettings["IdentifyingCodeMsgReg"];
                Msgs.Add(smstype, msgstr);
                return msgstr;
            }
            if ((int)SmsType.IdentifyingCode_ResetPwd == smstype)
            {
                string msgstr = ConfigurationManager.AppSettings["IdentifyingCodeMsgResetPwd"];
                Msgs.Add(smstype, msgstr);
                return msgstr;
            }
            if ((int)SmsType.Notice == smstype)
            {
                string msgstr = ConfigurationManager.AppSettings["NoticeMsg"];
                Msgs.Add(smstype, msgstr);
                return msgstr;
            }
            return null;
        }

        /// <summary>
        /// 发送短信，验证，处理
        /// </summary>
        /// <param name="phone">号码</param>
        /// <param name="smstype">短信类型</param>
        /// <param name="clientIp">客户端ip</param>
        /// <returns></returns>
        public ReturnItem<string> SendMessage(string Phone, string clientip, int smstype)
        {
            string msgcontent = GetMsgContent(smstype);
            if (msgcontent == null || msgcontent == "")
            {
                return new ReturnItem<string>() { Code = 0, Msg = "短信模板配置不正确" };
            }

            PhoneCodeBLL bll = new PhoneCodeBLL();
            PhoneCodeModel model = bll.GetPhoneCode(Phone, smstype);
            //短时间（1分钟）内重复获取
            if (model != null && model.StartTime.AddMinutes(1) > DateTime.Now)
            {
                return new ReturnItem<string>() { Code = 0, Msg = "请耐心等待，勿重复获取" };
            }

            //相同IP每天限制可以发送的数量
            if (clientip != "")
            {
                if (bll.GetCountByClientIpEveryDay(clientip) > Convert.ToInt32(GetIpSendNumber()))
                {
                    return new ReturnItem<string>() { Code = 0, Msg = "Ip发送次数超过限制，请明天再试" };
                }
            }

            //相同号码每天限制可以发送的数量
            if (Phone != "")
            {
                if (bll.GetCountByPhoneEveryDay(Phone) > Convert.ToInt32(GetPhoneSendNumber()))
                {
                    return new ReturnItem<string>() { Code = 0, Msg = "号码发送次数超过限制，请明天再试" };
                }
            }

            //如果验证码有效，还是发送上次的验证码
            string code = "";
            if (model == null)
            {
                System.Random Random = new System.Random();
                int Result = Random.Next(0, 999999);
                while (Result.ToString().Length < 6)
                {
                    Result = Random.Next(0, 999999);
                }
                model = new PhoneCodeModel();
                model.Code = Result.ToString();
                model.Phone = Phone;
                model.ClientIp = clientip;
                model.StartTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddMinutes(10);
                model.SmsType = smstype.ToString();
                model.SmsContent = msgcontent;
                bll.InsertPhoneCode(model);

            }
            code = model.Code;

            if (GetSmsIsDebug() == "1")
            {
                return new ReturnItem<string>() { Code = 1, Msg = "验证码发送成功" };
            }
            else
            {
                //发送短信
                QDBeaconFire fire = new QDBeaconFire();
                string ret = fire.Send(Phone, msgcontent.Replace("!!!!!!", code));

                var xml = System.Xml.Linq.XElement.Parse(ret);
                if (xml.Elements("returnstatus").FirstOrDefault().Value == "Success")
                {
                    ret = "验证码发送成功";
                }
                else
                {
                    ret = "服务器内部错误，请稍候重试。";
                    log.ErrorFormat("[SMS]发送短信失败：{0}。", xml.Elements("message").FirstOrDefault().Value);
                }

               
                return new ReturnItem<string>() { Code = 1, Msg = ret };
            }
        }

        public string[] GetMsgAccountInfo()
        {
            //cust_code: 570063,status: 1,sms_balance: 20
            //status表示：
            //0 账号已停用
            //1 账号在用
            //2 账号处在测试状态

            QDBeaconFire fire = new QDBeaconFire();
            string ret = fire.GetAccount();

            string[] temp = ret.Split(',');
            List<string> rets = new List<string>();
            rets.Add(temp[0].Split(':')[1]);
            string temptt = temp[1].Split(':')[1];
            if (temptt == "0")
            {
                rets.Add("账号已停用");
            }
            else if (temptt == "1")
            {
                rets.Add("账号在用");
            }
            else if (temptt == "2")
            {
                rets.Add("账号处在测试状态");
            }
            else
            {
                rets.Add("未知状态");
            }
            rets.Add(temp[2].Split(':')[1]);
            return rets.ToArray();
        }
    }
}
