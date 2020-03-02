using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBLL.SMS
{
    public class QDBeaconFire
    {
        private static string cust_code = ConfigurationManager.AppSettings["cust_code"];
        private static string sp_code = ConfigurationManager.AppSettings["sp_code"];
        private static string cust_pwd = ConfigurationManager.AppSettings["cust_pwd"];
        private static string userid = ConfigurationManager.AppSettings["userid"];

        private static string httpip = ConfigurationManager.AppSettings["QDBeaconFireIp"];
        private static string httpport = ConfigurationManager.AppSettings["QDBeaconFirePort"];
        private static string httpaddress = "http://" + httpip + ":" + httpport + "/sms.aspx";
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="destMobiles">目标号码</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string Send(string destMobiles, string content)
        {
            //cust_code = ***&sp_code = ***&content = ****&destMobiles = ***,****,***&sign = ****
            string poststr = "action=send&userid="+ userid + "&account="+ cust_code + "&password="+ cust_pwd + "&content="+ content + "&mobile=" + destMobiles;
            string ret = IndHttpRequest.HttpPost(httpaddress, poststr);
            return Uri.UnescapeDataString(ret);
        }
        /// <summary>
        /// 获取token值
        /// </summary>
        /// <returns></returns>
        public List<string> GetToken()
        {
            string poststr = httpaddress + "/?action=GetToken&cust_code=" + cust_code;
            string ret = IndHttpRequest.HttpGet(poststr);
            return ret.Split(',').ToList();
        }
        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <returns></returns>
        public string GetAccount()
        {
            List<string> list = GetToken();
            string token_id = list[0].Split(':').ToList()[1];
            string token = list[1].Split(':').ToList()[1];
            //cust_code = ***&sp_code = ***&content = ****&destMobiles = ***,****,***&sign = ****
            string poststr = httpaddress + "/?action=QueryAccount&cust_code=" + cust_code + "&token_id=" + token_id +
                "&sign=" + CommonTool.GetMd5(token + cust_pwd);
            string ret = IndHttpRequest.HttpGet(poststr);
            return ret;
        }
    }
}
