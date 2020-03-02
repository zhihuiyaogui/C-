using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonTool
    {
        /// <summary>
        /// MD5 hash加密(移自老项目)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(s.Trim())));
            return result;
        }
        public static string GetMd5(string source)
        {
            //获取要加密的字段，并转化为Byte[]数组  
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source.ToCharArray());
            //建立加密服务  
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //加密Byte[]数组  
            byte[] result = md5.ComputeHash(data);
            //将加密后的数组转化为字段  
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in result)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            return s.ToString();
        }
        //	当国际为中国时，手机开头必须为133、153、180、181、189、130、131、132、155、156、185、186、134、135、136、137、138、139、150、151、152、158、159、182、183、184、157、187、188、176、185、177、147、178、170、145，否则提示格式错误
        /// <summary>
        /// 验证国内手机号的逻辑
        /// </summary>
        /// <param name="phone">长度11</param>
        /// <returns></returns>
        public static bool CheckChinaPhonenNumber(string phone)
        {
            bool r = false;
            string headstr = "133,153,180,181,189,130,131,132,155,156,185,186,134,135,136,137,138,139,150,151,152,158,159,182,183,184,157,187,188,176,185,177,147,178,170,145";//后续可移动到Web.config
            List<string> head = headstr.Split(',').ToList();
            if (!checkPhoneLength(phone, 11))
            {
                r = false;//长度不够
                return r;
            }
            if (!head.Contains(phone.Substring(0, 3)))
            {
                r = false;//号码否不符合约定
                return r;
            }
            r = true;
            return r;
        }
        static bool checkPhoneLength(string phone, int length)
        {
            bool r = true;
            if (phone.TrimEnd().Length != length)
            {
                r = false;//长度不够
            }
            return r;
        }
        /// <summary>
        /// 检查泰国的手机号（长度9）
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool CheckThailandPhoneNumber(string phone)
        {
            bool r = false;
            if (!checkPhoneLength(phone, 9))
            {
                r = false;//长度不够
                return r;
            }
            r = true;
            return r;
        }
        /// <summary>
        /// 获取随机数验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }
    }
}
