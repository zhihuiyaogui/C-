using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Common.quanlangJson;
using System.Net.Http;
using GenerSoft.OpenTSDB.Client;
using System.Threading;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    public class ControllDeviceBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static byte[] buffer = new byte[2048];
        private static int count = 0;
        private static Dictionary<string, Socket> deviceIdToSocket = new Dictionary<string, Socket>();
        private static Dictionary<string, RootObject> lastestDeviceInfo = new Dictionary<string, RootObject>();
        private static Dictionary<string, DateTime> lastestDeviceInfoTime = new Dictionary<string, DateTime>();//用以记录上一条记录的时间
        private static Dictionary<string, bool> isDataChange = new Dictionary<string, bool>();//标识某个设备的新一条数据是否到了，设备ID的最后四位作为key
        private static Dictionary<string, int> countToStore = new Dictionary<string, int>();//计数，对于某设备ID下的数据intervalToStore次存一次
        private const int sleepTime = 200;//一次等待200毫秒
        private const int sleepNum = 10;//最多sleep10次也就是2秒
        private const int intervalToStore = 20;//socket数据隔多少次存一次
        public static string opentsdburl = "http://192.168.1.122:4242";

        public ReturnItem<RootObject> ControllDevice(command c)
        {
            try
            {
                if (c.cmd != "power")//不是进行开关机
                {
                    //发送指令之前的设备指令状态
                    RootObject pre = new RootObject();
                    pre = getLastestDeviceInfoInner(c);
                    bool isChangePre = isDataChange[c.deviceId];

                    ReturnItem<RootObject> r = new ReturnItem<RootObject>();
                    Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
                    //遍历获取到key对应的socket
                    foreach (string key in keyCol)
                    {
                        if (key.IndexOf(c.deviceId) > -1)
                        {
                            var socket = deviceIdToSocket[key];
                            var outputBuffer = Encoding.UTF8.GetBytes(ToJson(c.cmd, c.value));
                            socket.Send(outputBuffer);
                        }
                    }
                    //最多循环等待sleepNum次，
                    for (int i = 0; i < sleepNum; i++)
                    {
                        bool isChangeAfter = isDataChange[c.deviceId];
                        if (isChangeAfter != isChangePre)
                            break;
                        Thread.Sleep(sleepTime);//睡200毫秒
                    }
                    RootObject after = new RootObject();
                    after = getLastestDeviceInfoInner(c);
                    //判断发送指令前后设备状态是否相同
                    if (judgeIsDeviceInfoSame.isSame(pre, after))
                    {
                        //上面的判断是确保前后两条消息相同代表指令发送失败，但是也不准确。例如当我们的控制指令发出以后，设备新的一条消息也发出。这时肯定两条指令相同，
                        //发送失败，但其实未必。所以再接受一条消息，如果还是不同则失败会被判定为指令发送失败
                        for (int i = 0; i < sleepNum; i++)
                        {
                            bool isChangeAfter = isDataChange[c.deviceId];
                            if (isChangeAfter == isChangePre)//再接收一条消息则进行相同判定
                                break;
                            Thread.Sleep(sleepTime);//睡200毫秒
                        }
                        after = getLastestDeviceInfoInner(c);
                        if (judgeIsDeviceInfoSame.isSame(pre, after))
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "指令发送失败";
                            return r;
                        }
                    }
                    r.Data = after;
                    r.Code = 0;
                    r.Msg = "指令发送成功";
                    return r;
                }
                else
                {
                    //发送指令之前的设备指令状态
                    RootObject pre = new RootObject();
                    pre = getLastestDeviceInfoInner(c);
                    bool isChangePre = isDataChange[c.deviceId];

                    ReturnItem<RootObject> r = new ReturnItem<RootObject>();
                    Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
                    //遍历获取到key对应的socket
                    foreach (string key in keyCol)
                    {
                        if (key.IndexOf(c.deviceId) > -1)
                        {
                            var socket = deviceIdToSocket[key];
                            var outputBuffer = Encoding.UTF8.GetBytes(ToJson(c.cmd, c.value));
                            socket.Send(outputBuffer);
                        }
                    }

                    //最多循环等待sleepNum次
                    for (int i = 0; i < sleepNum; i++)
                    {
                        bool isChangeAfter = isDataChange[c.deviceId];
                        if (isChangeAfter != isChangePre)
                            break;
                        Thread.Sleep(sleepTime);//睡200毫秒
                    }
                    RootObject after = new RootObject();
                    after = getLastestDeviceInfoInner(c);

                    //判断发送指令前后设备状态是否相同
                    if (pre.dp_value.power == after.dp_value.power)
                    {
                        //上面的判断是确保前后两条消息相同代表指令发送失败，但是也不准确。例如当我们的控制指令发出以后，设备新的一条消息也发出。这时肯定两条指令相同，会被判定为指令
                        //发送失败，但其实未必。所以再接受一条消息，如果还是不同则失败
                        for (int i = 0; i < sleepNum; i++)
                        {
                            bool isChangeAfter = isDataChange[c.deviceId];
                            if (isChangeAfter == isChangePre)//再接收一条消息则进行相同判定
                                break;
                            Thread.Sleep(sleepTime);//睡200毫秒
                        }
                        after = getLastestDeviceInfoInner(c);
                        if (pre.dp_value.power == after.dp_value.power)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "指令发送失败";
                            return r;
                        }
                    }
                    r.Data = after;
                    r.Code = 0;
                    r.Msg = "指令发送成功";
                    return r;
                }
            }
            catch
            {
                return null;
            }


        }

        public ReturnItem<RootObject> ControllDeviceList(command c)
        {
            if (c.cmd != "power")
            {
                ReturnItem<RootObject> r = new ReturnItem<RootObject>();
                foreach (string deviceId in c.deviceIdList)
                {
                    Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
                    //遍历获取到key对应的socket
                    foreach (string key in keyCol)
                    {
                        if (key.IndexOf(deviceId) > -1)
                        {
                            var socket = deviceIdToSocket[key];
                            var outputBuffer = Encoding.UTF8.GetBytes(ToJson(c.cmd, c.value));
                            socket.Send(outputBuffer);
                        }
                    }
                }
                r.Data = null;
                r.Code = 0;
                r.Msg = "指令发送成功";
                return r;
            }
            else
            {
                ReturnItem<RootObject> r = new ReturnItem<RootObject>();
                foreach (string deviceId in c.deviceIdList)
                {
                    Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
                    //遍历获取到key对应的socket
                    foreach (string key in keyCol)
                    {
                        if (key.IndexOf(deviceId) > -1)
                        {
                            var socket = deviceIdToSocket[key];
                            var outputBuffer = Encoding.UTF8.GetBytes(ToJson(c.cmd, c.value));
                            socket.Send(outputBuffer);
                        }
                    }
                }
                r.Data = null;
                r.Code = 0;
                r.Msg = "指令发送成功";
                return r;
            }

        }

        //获取最近一次设备信息，外部调用
        public ReturnItem<RootObject> getLastestDeviceInfoOuter(command c)
        {
            ReturnItem<RootObject> r = new ReturnItem<RootObject>();
            Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
            //遍历获取到key对应的socket
            foreach (string key in keyCol)
            {
                if (key.IndexOf(c.deviceId) > -1)
                {
                    DateTime now = DateTime.Now;
                    DateTime datatime = lastestDeviceInfoTime[key];
                    double interval = (now - datatime).TotalSeconds;
                    if (interval < 20)
                    {
                        r.Data = lastestDeviceInfo[key];
                        r.Code = 0;
                        r.Msg = "获取最近一次风机设备信息成功";
                        return r;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            r.Data = null;
            r.Code = -1;
            r.Msg = "风机的最近一次数据不存在";
            return r;
        }
        //获取最近一次设备信息，内部调用
        public RootObject getLastestDeviceInfoInner(command c)
        {
            try
            {
                Dictionary<string, Socket>.KeyCollection keyCol = deviceIdToSocket.Keys;
                //遍历获取到key对应的socket
                foreach (string key in keyCol)
                {
                    if (key.IndexOf(c.deviceId) > -1)
                    {
                        return lastestDeviceInfo[key];
                    }
                }
            }
            catch
            {

            }
            return null;
        }
        /*
         下面的两个函数用于在程序启动时异步启动
        */
        public async static void initSocket()
        {
            string res = await Run();
            return;
        }
        private static Task<string> Run()
        {
            return Task.Run(() => { init(); return ""; });
        }
        /// <summary>
        /// 初始化socket链接
        /// </summary>
        public static void init()
        {
            log.Info("server:ready");
            #region 启动程序
            //①创建一个新的Socket,这里我们使用最常用的基于TCP的Stream Socket（流式套接字）
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //②将该socket绑定到主机上面的某个端口
            string s = GetLocalIP();
            IPAddress ip = IPAddress.Parse(s);//192.168.137.48
            socket.Bind(new IPEndPoint(ip, 9006));

            //③启动监听，并且设置一个最大的队列长度
            socket.Listen(10000);

            //④开始接受客户端连接请求
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);

        }
        #region 客户端连接成功
        /// <summary>
        /// 客户端连接成功
        /// </summary>
        /// <param name="ar"></param>
        public static void ClientAccepted(IAsyncResult ar)
        {
            #region
            //设置计数器
            count++;
            var socket = ar.AsyncState as Socket;

            //这就是客户端的Socket实例，我们后续可以将其保存起来
            var client = socket.EndAccept(ar);
            //客户端IP地址和端口信息
            IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint;
            log.InfoFormat("{0} is connected，total connects {1}", clientipe, count);
            try
            {
                //接收客户端的消息(这个和在客户端实现的方式是一样的）异步
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            }
            catch (Exception e)
            {
                log.InfoFormat("socket连接错误:", e);
            }

            //准备接受下一个客户端请求(异步)
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);
            #endregion
        }
        #endregion

        #region 接收客户端的信息
        /// <summary>
        /// 接收某一个客户端的消息
        /// </summary>
        /// <param name="ar"></param>
        public static void ReceiveMessage(IAsyncResult ar)
        {
            int length = 0;
            string message = "";
            var socket = ar.AsyncState as Socket;
            //客户端IP地址和端口信息
            IPEndPoint clientipe = (IPEndPoint)socket.RemoteEndPoint;
            try
            {
                #region
                length = socket.EndReceive(ar);
                //读取出来消息内容
                message = Encoding.UTF8.GetString(buffer, 0, length);
                /*
                {
                    "dp_value":
                    {   
                        "ID": "M600_B0F893195040", 
                        "fanspeed": 1, 
                        "fresh": 1, 
                        "heat": 0, 
                        "power": 1, 
                        "sleep": 0, 
                        "auto": 0, 
                        "childlock": 0, 
                        "lcd": 0, 
                        "tempin": "+151", 
                        "tempout": "+151", 
                        "co2": 0, 
                        "pm25": 0, 
                        "sur1": 64505, "total1": 72000, 
                        "sur2": 82505, "total2": 90000, 
                        "sur3": 82505, "total3": 90000 
                     }
                  }
                */
                //解析消息（json数据）
                RootObject dp = new RootObject();
                try
                {
                    dp = JsonConvert.DeserializeObject<RootObject>(message);
                    log.InfoFormat("dp.ID:    " + dp.dp_value.ID);
                }
                catch (Exception e)
                {
                    log.InfoFormat("json数据解析错误:{1}", e);
                }

                //将解析出来的设备ID的最后四位作为key
                string nowId = dp.dp_value.ID.Substring(dp.dp_value.ID.Length - 4);

                //来一条数据就进行计数
                if (!countToStore.ContainsKey(nowId))
                {
                    countToStore.Add(nowId, 0);
                }
                else
                {
                    if (countToStore[nowId] < intervalToStore)
                    {
                        countToStore[nowId]++;
                    }
                    else
                    {
                        countToStore[nowId] = 0;
                        //将解析出来的数据存入openTSDB
                        dp.dp_value.ID = nowId;//ID最后四位存入TSDB
                        PutDataToTSDB(dp);
                    }

                }

                //将对应设备的最新一条数据是否来到的标志量进行改变
                if (!isDataChange.ContainsKey(nowId))
                {
                    isDataChange.Add(nowId, false);
                }
                else
                {
                    if (isDataChange[nowId] == false)
                    {
                        isDataChange[nowId] = true;
                    }
                    else
                    {
                        isDataChange[nowId] = false;
                    }

                }

                //将设备的最近一次的设备数据存起来
                if (lastestDeviceInfo.ContainsKey(nowId))
                {
                    lastestDeviceInfo[nowId] = dp;
                    lastestDeviceInfoTime[nowId] = DateTime.Now;
                }
                else
                {
                    lastestDeviceInfo.Add(nowId, dp);
                    lastestDeviceInfoTime.Add(nowId, DateTime.Now);
                }
                //以设备id为key，每次设备来数据都把这个socket存起来
                if (deviceIdToSocket.ContainsKey(nowId))
                {
                    deviceIdToSocket[nowId] = socket;
                }
                else
                {
                    deviceIdToSocket.Add(nowId, socket);
                }
                //服务器发送消息
                //var outputBuffer = Encoding.UTF8.GetBytes(ToJson("filter", "2"));
                //socket.Send(outputBuffer);

                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息）异步
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
                #endregion
            }
            catch (Exception ex)
            {
                //设置计数器
                count--;
                //断开连接
                log.InfoFormat("{0} is disconnected，total connects {1}", clientipe, count);
            }
        }
        public static void PutDataToTSDB(RootObject dp)
        {
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            string datatime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            var realDatatime = DateTimeUtil.parse(datatime, "yyyyMMdd HH:mm:ss");
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("deviceId", dp.dp_value.ID);
                if (dp.dp_value.power.Equals("0"))
                {
                    client.putData("power", realDatatime, dp.dp_value.power, tagMap);
                }
                else
                {
                    client.putData("fanspeed", realDatatime, dp.dp_value.fanspeed, tagMap);
                    client.putData("fresh", realDatatime, dp.dp_value.fresh, tagMap);
                    client.putData("heat", realDatatime, dp.dp_value.heat, tagMap);
                    client.putData("power", realDatatime, dp.dp_value.power, tagMap);
                    client.putData("sleep", realDatatime, dp.dp_value.sleep, tagMap);
                    client.putData("auto", realDatatime, dp.dp_value.auto, tagMap);
                    client.putData("childlock", realDatatime, dp.dp_value.childlock, tagMap);
                    client.putData("lcd", realDatatime, dp.dp_value.lcd, tagMap);
                    client.putData("tempin", realDatatime, dp.dp_value.tempin, tagMap);
                    client.putData("tempout", realDatatime, dp.dp_value.tempout, tagMap);
                    int s;
                    int t;
                    s = int.Parse(dp.dp_value.sur1);
                    t = int.Parse(dp.dp_value.total1);
                    client.putData("sur1", realDatatime, (s / t).ToString(), tagMap);
                    s = int.Parse(dp.dp_value.sur2);
                    t = int.Parse(dp.dp_value.total2);
                    client.putData("sur1", realDatatime, (s / t).ToString(), tagMap);
                    s = int.Parse(dp.dp_value.sur3);
                    t = int.Parse(dp.dp_value.total3);
                    client.putData("sur1", realDatatime, (s / t).ToString(), tagMap);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //获取本机的iPv4地址
        #endregion
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                log.Info("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
        public static string ToJson(string parameter, string value)
        {
            /*
                {
                    "dp_value":
                    {   
                        "ID": "M600_B0F893195040", 
                        "fanspeed": 1, 
                        "fresh": 1, 
                        "heat": 0, 
                        "power": 1, 
                        "sleep": 0, 
                        "auto": 0, 
                        "childlock": 0, 
                        "lcd": 0, 
                        "tempin": "+151", 
                        "tempout": "+151", 
                        "co2": 0, 
                        "pm25": 0, 
                        "sur1": 64505, "total1": 72000, 
                        "sur2": 82505, "total2": 90000, 
                        "sur3": 82505, "total3": 90000 
                     }
                  }
                */
            JObject obj = new JObject();
            switch (parameter)
            {
                case "heat":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        heater = int.Parse(value)
                    }));
                    break;
                case "power":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        power = int.Parse(value)
                    }));
                    break;
                case "childlock":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        childlock = int.Parse(value)
                    }));
                    break;
                case "lcd":
                    if (value == "1")
                    {
                        obj.Add("dp_value", JToken.FromObject(new
                        {
                            lcd = "on"
                        }));
                    }
                    else
                    {
                        obj.Add("dp_value", JToken.FromObject(new
                        {
                            lcd = "off"
                        }));
                    }
                    break;
                case "fresh":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        fresh = int.Parse(value)
                    }));
                    break;
                case "auto":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        auto = int.Parse(value)
                    }));
                    break;
                case "fanspeed":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        speed = int.Parse(value)
                    }));
                    break;
                case "sleep":
                    obj.Add("dp_value", JToken.FromObject(new
                    {
                        sleep = int.Parse(value)
                    }));
                    break;
                case "filter":
                    if (Regex.IsMatch(value, @"^[-]?\d+[.]?\d*$"))
                    {
                        obj.Add("dp_value", JToken.FromObject(new
                        {
                            filter = int.Parse(value)
                        }));
                    }
                    else
                    {
                        obj.Add("dp_value", JToken.FromObject(new
                        {
                            filter = value
                        }));
                    }
                    break;
            }

            //合并其他对象到当前对象的属性
            return obj.ToString();
        }

    }
}
#endregion