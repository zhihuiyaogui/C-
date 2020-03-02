using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class EncryptHelper
    {
        /// <summary>
        /// 生成随即加密key 7
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string getKey(this string s)
        {
            string[] estr = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            //var num = RandCode(let);
            StringBuilder sb = new StringBuilder();
            Random ran = new Random();
            bool run = true;
            while (run)
            {
                int n = ran.Next(0, estr.Length);
                if (sb.ToString().ToArray().Select(ci => Convert.ToString(ci)).ToList().Where(si => si == estr[n]).Count() <= 3)
                {
                    sb.Append(estr[n]);
                    run = !(sb.ToString().ToArray().Length == 16);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 把文本Aes加密再用base64输出
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToAesEncryptString(this string toEncrypt, string key, string iv)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// 解密Aes数据到普通文本
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string ToAesDecrypt(this string toDecrypt, string key, string iv)
        {
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.Zeros;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                var getstr = UTF8Encoding.UTF8.GetString(resultArray).Replace("\0", "");
                return getstr;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 解密Json字符串并反序列化到指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T AesDecryptToEntity<T>(this string toDecrypt, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(toDecrypt)) { return default(T); }
            else
            {
                return JsonConvert.DeserializeObject<T>(ToAesDecrypt(toDecrypt, key, iv));
            }
        }
        /// <summary>  
        /// Base64加密  
        /// </summary>  
        /// <param name="Message"></param>  
        /// <returns></returns>  
        public static string ToBase64EncryptString(this string Message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Message);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>  
        /// Base64解密  
        /// </summary>  
        /// <param name="Message"></param>  
        /// <returns></returns>  
        public static string ToBase64DecryptString(this string Message)
        {
            if (string.IsNullOrWhiteSpace(Message)) { return ""; }
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
