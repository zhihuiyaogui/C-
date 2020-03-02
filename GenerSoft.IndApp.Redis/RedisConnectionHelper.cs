/****************************************************************
*   作	者：丁维理
*
*   CLR版本：4.5.2
*
*   文件版本：1.0
*
*   创建时间：2018/04/19 22:59:57
*
*   描述说明：
*
*   修改历史：
*
*   文件名： RedisConnectionHelper.cs
*
*   Copyright (c) 2017 chuxin Corporation. All rights reserved.
*****************************************************************/
using Common.Config;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuXin.Redis
{
    public class RedisConnectionHelper
    {
        private static readonly object Locker = new object();
        private static ConnectionMultiplexer _instance;
        /// <summary>
        /// 单例获取Redis多路连接器
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetManager();
                        }
                    }
                }
                return _instance;
            }
        }
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache =
              new ConcurrentDictionary<string, ConnectionMultiplexer>();
        /// <summary>
        /// 缓存获取Redis多路连接器
        /// </summary>
        /// <param name="connectionString">WebConfig下appSettings => RedisConnectString 字段</param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            connectionString = connectionString ?? CustomConfigParam.RedisConnectString;
            var connect = ConnectionMultiplexer.Connect(connectionString);
            return connect;
        }
    }
}
