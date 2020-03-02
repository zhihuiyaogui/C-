using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    public class CustomConfigParam
    {
        /// <summary>
        /// 是否使用Redis
        /// </summary>
        public static bool IsUseRedis { get {
                if (!_IsReadRedis)
                {
                    _IsUseRedis = Convert.ToBoolean(ConfigurationManager.AppSettings["IsUseRedis"]);//是否使用Redis
                    _IsReadRedis = true;
                }
                return _IsUseRedis;
            }
        }
        private static bool _IsReadRedis = false;
        private static bool _IsUseRedis = false;

        /// <summary>
        /// 是否加密传输
        /// </summary>
        public static bool IsEncrypt { get {
                if (!_IsReadEncrypt)
                {
                    _IsEncrypt = Convert.ToBoolean(ConfigurationManager.AppSettings["IsEncrypt"]);//是否加密传输
                    _IsReadEncrypt = true;
                }

                return _IsEncrypt;
            } }
        private static bool _IsReadEncrypt = false;
        private static bool _IsEncrypt = false;

        /// <summary>
        /// Redis数据库号
        /// </summary>
        public static int RedisDbNumber
        {
            get
            {
                if (!_IsReadRedisDbNumber)
                {
                    _RedisDbNumber = ConfigurationManager.AppSettings["RedisDbNumber"] != null? Convert.ToInt32(ConfigurationManager.AppSettings["RedisDbNumber"]):0;//Redis数据库号
                    _IsReadRedisDbNumber = true;
                }

                return _RedisDbNumber;
            }
        }
        private static bool _IsReadRedisDbNumber = false;
        private static int _RedisDbNumber = 0;

        /// <summary>
        /// 模块Id
        /// </summary>
        public static string ModuleId
        {
            get
            {
                if (!_IsReadModuleId)
                {
                    _ModuleId = ConfigurationManager.AppSettings["ModuleId"];//模块Id
                    _IsReadModuleId = true;
                }

                return _ModuleId;
            }
        }
        private static bool _IsReadModuleId = false;
        private static string _ModuleId = "";

        /// <summary>
        /// Api的Token
        /// </summary>
        public static string WebApiToken
        {
            get
            {
                if (!_IsReadWebApiToken)
                {
                    _WebApiToken = ConfigurationManager.AppSettings["WebApiToken"];//Api的Token
                    _IsReadWebApiToken = true;
                }

                return _WebApiToken;
            }
        }
        private static bool _IsReadWebApiToken = false;
        private static string _WebApiToken = "";

        /// <summary>
        /// 上传文件路径
        /// </summary>
        public static string UploadPath
        {
            get
            {
                if (!_IsReadUploadPath)
                {
                    _UploadPath = ConfigurationManager.AppSettings["UploadPath"];//上传文件路径
                    _IsReadUploadPath = true;
                }

                return _UploadPath;
            }
        }
        private static bool _IsReadUploadPath = false;
        private static string _UploadPath = "";

        /// <summary>
        /// 头像上传文件路径
        /// </summary>
        public static string HeadPicUploadPath
        {
            get
            {
                if (!_IsReadHeadPicUploadPath)
                {
                    _HeadPicUploadPath = ConfigurationManager.AppSettings["HeadPicUploadPath"];//上传文件路径
                    _IsReadHeadPicUploadPath = true;
                }

                return _HeadPicUploadPath;
            }
        }
        private static bool _IsReadHeadPicUploadPath = false;
        private static string _HeadPicUploadPath = "";

        /// <summary>
        /// 头像访问Url路径
        /// </summary>
        public static string HeadPicBaseUrl
        {
            get
            {
                if (!_IsReadHeadPicBaseUrl)
                {
                    _HeadPicBaseUrl = ConfigurationManager.AppSettings["HeadPicBaseUrl"];//上传文件路径
                    _IsReadHeadPicBaseUrl = true;
                }

                return _HeadPicBaseUrl;
            }
        }
        private static bool _IsReadHeadPicBaseUrl = false;
        private static string _HeadPicBaseUrl = "";

        /// <summary>
        /// Redis超时时间
        /// </summary>
        public static string RedisExpiryHours
        {
            get
            {
                if (!_IsReadRedisExpiryHours)
                {
                    _RedisExpiryHours = ConfigurationManager.AppSettings["RedisExpiryHours"];//Redis超时时间
                    _IsReadRedisExpiryHours = true;
                }

                return _RedisExpiryHours;
            }
        }
        private static bool _IsReadRedisExpiryHours = false;
        private static string _RedisExpiryHours = "";

        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public static string RedisConnectString
        {
            get
            {
                if (!_IsReadRedisConnectString)
                {
                    _RedisConnectString = ConfigurationManager.AppSettings["RedisConnectString"];//Redis连接字符串
                    _IsReadRedisConnectString = true;
                }

                return _RedisConnectString;
            }
        }
        private static bool _IsReadRedisConnectString = false;
        private static string _RedisConnectString = "";

        /// <summary>
        /// 用户模块API地址
        /// </summary>
        public static string UserApiUrl
        {
            get
            {
                if (!_IsReadUserApiUrl)
                {
                    _UserApiUrl = ConfigurationManager.AppSettings["UserApiUrl"];//用户模块API地址
                    _IsReadUserApiUrl = true;
                }

                return _UserApiUrl;
            }
        }
        private static bool _IsReadUserApiUrl = false;
        private static string _UserApiUrl = "";

        /// <summary>
        /// 设备模块API地址
        /// </summary>
        public static string DeviceApiUrl
        {
            get
            {
                if (!_IsReadDeviceApiUrl)
                {
                    _DeviceApiUrl = ConfigurationManager.AppSettings["DeviceApiUrl"];//设备模块API地址
                    _IsReadDeviceApiUrl = true;
                }

                return _DeviceApiUrl;
            }
        }
        private static bool _IsReadDeviceApiUrl = false;
        private static string _DeviceApiUrl = "";

        /// <summary>
        /// 报警模块API地址
        /// </summary>
        public static string AlertApiUrl {
            get
            {
                if (!_IsReadAlertApiUrl)
                {
                    _AlertApiUrl = ConfigurationManager.AppSettings["AlertApiUrl"];//报警模块API地址
                    _IsReadAlertApiUrl = true;
                }
                return _AlertApiUrl;
            }
        }

        private static bool _IsReadAlertApiUrl = false;
        private static string _AlertApiUrl = "";

        /// <summary>
        /// 接口时间戳间隔时间配置 分钟
        /// </summary>
        public static double ApiTimeStamp
        {
            get
            {
                if (!_IsReadApiTimeStamp)
                {
                    _ApiTimeStamp = ConfigurationManager.AppSettings["ApiTimeStamp"];//接口时间戳间隔时间配置
                    _IsReadApiTimeStamp = true;
                }
                try
                {
                    return Convert.ToDouble(_ApiTimeStamp);
                }
                catch
                {
                    return 0;
                }

            }
        }
        private static bool _IsReadApiTimeStamp = false;
        private static string _ApiTimeStamp = "";


        /// <summary>
        /// 是否启用MQTT服务
        /// </summary>
        public static bool EnableMqtt
        {
            get
            {
                if (!_isReadMqtt)
                {
                    _EnableMqtt = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMqtt"]);
                    _isReadMqtt = true;
                }
                return _EnableMqtt;
            }
        }

        private static bool _isReadMqtt = false;
        private static bool _EnableMqtt = false;
    }
}
