using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GenerSoft.OpenTSDB.Client;
using System.Timers;
using GenerSoft.IndApp.AlertPoliciesBLL;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.AlertPoliciesDAL;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.HistoryAlertPolicies;
using GenerSoft.IndApp.CommonSdk.Model.User;
using DeviceMonitoringDAL;

namespace AlertPoliciesBLL
{
    public class storeAirData2Opentsdb
    {
        public async static void getDataServiceStart()
        {
            string res = await Run();
            return;
        }

        private static Task<string> Run()
        {
            return Task.Run(() => { start(); return ""; });
        }
        //预设一个设备数量
        readonly static int deviceSum = 100;
        //每个设备上次插入opentsdb的时间戳
        private static string[] lastTimeStamp = new string[deviceSum];
        public static void start()
        {
            PutAirData();
            Timer timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 60000; //执行间隔时间,单位为毫秒; 这里实际间隔为1分钟  
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(test);

            Console.ReadKey();
        }

        private static void test(object source, ElapsedEventArgs e)
        {
            PutAirData();
        }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string opentsdburl = "http://192.168.1.122:4242";
        public static int intervalToAlert = 20;//来intervalToAlert次数据去检测一次是否触发报警
        public static int nowInterval = 0;//目前已经来了几次
        public static void PutAirData()
        {
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            string phone = "15064113079";
            string password = "sduasp";
            try
            {
                //获取空气数据的列表
                List<airData> airDataList = GetSensorDataByApiBLL.getAirData(phone, password);
                int numPutToTSDB = 0;//这一次for循环存的设备数
                for (int i = 0; i < airDataList.Count; i++)
                {
                    //以下if用于判断i设备的数据时间戳是否变化，没变化不再执行写入
                    if (lastTimeStamp[i] == null)
                    {
                        lastTimeStamp[i] = airDataList[i].data[0].timestamp;
                        numPutToTSDB++;
                    }
                    else
                    {
                        if (lastTimeStamp[i].Equals(airDataList[i].data[0].timestamp))
                        {
                            continue;
                        }
                        else
                        {
                            lastTimeStamp[i] = airDataList[i].data[0].timestamp;
                            numPutToTSDB++;
                    }
                    }
                    Dictionary<string, string> tagMap = new Dictionary<string, string>();
                    tagMap.Add("deviceId", airDataList[i].deviceId);
                    string datatime = ConvertToTime(airDataList[i].data[0].timestamp).ToString("yyyyMMdd HH:mm:ss");
                    var realDatatime = DateTimeUtil.parse(datatime, "yyyyMMdd HH:mm:ss");
                    //public bool putData(string metric, DateTime timestamp, string value, Dictionary<string, string> tagMap)
                    // AsyncHandleAlarmPolicies1(string deviceId, string air, string curValue, DateTime now)
                    
                    client.putData("pm2d5", realDatatime, airDataList[i].data[0].data.pm2d5, tagMap);

                    client.putData("pm10", realDatatime, airDataList[i].data[0].data.pm10, tagMap);

                    client.putData("temperature", realDatatime, airDataList[i].data[0].data.temperature, tagMap);

                    client.putData("humidity", realDatatime, airDataList[i].data[0].data.humidity, tagMap);

                    client.putData("pm1d0", realDatatime, airDataList[i].data[0].data.pm1d0, tagMap);

                    client.putData("co2", realDatatime, airDataList[i].data[0].data.co2, tagMap);

                    double air = double.Parse(airDataList[i].data[0].data.tvoc);
                    air = air * 92.14 / 22400;//单位ppb转mg/m3的方法
                    client.putData("tvoc", realDatatime, air.ToString() , tagMap);

                    client.putData("ch2o", realDatatime, airDataList[i].data[0].data.ch2o, tagMap);

                    client.putData("rssi", realDatatime, airDataList[i].data[0].data.rssi, tagMap);

                    if(nowInterval >= intervalToAlert)
                    {
                        alert al = new alert();
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "pm2d5", airDataList[i].data[0].data.pm2d5, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "pm10", airDataList[i].data[0].data.pm10, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "temperature", airDataList[i].data[0].data.temperature, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "humidity", airDataList[i].data[0].data.humidity, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "pm1d0", airDataList[i].data[0].data.pm1d0, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "co2", airDataList[i].data[0].data.co2, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "tvoc", air.ToString(), realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "ch2o", airDataList[i].data[0].data.ch2o, realDatatime);
                        al.AsyncHandleAlarmPolicies1(airDataList[i].deviceId, "rssi", airDataList[i].data[0].data.rssi, realDatatime);
                    }
                }
                log.InfoFormat("{0}devices put to TSDB this time!", numPutToTSDB);
                nowInterval++;//执行完一次for循环就算存了一次数据
                if (nowInterval > intervalToAlert)
                {
                    nowInterval = 0;
                }
            }
            catch (Exception e)
            {
                
                log.Info("采集程序出异常啦！" + e.Message);
                
                // storeairdata2opentsdb.getdataservicestart();
            }
        }
        private class alert
        {
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            /// <summary>
            /// 处理报警逻辑(汉王数据接入使用)
            /// 1.是否命中报警策略
            /// 2.记录流水
            /// 3.发送消息
            /// </summary>
            /// <param name="deviceInfo"></param>
            /// <param name="itemInfo"></param>
            /// <param name="curValue"></param>
            /// <param name="now"></param>
            /// <returns></returns>
            private Task<int> HandleAlarmPolicies1(RetDeviceInfo deviceInfo, RetDeviceItemInfo itemInfo, string curValue, DateTime now)
            {
                int result = -1;
                using (AlertPoliciesEntities alert = new AlertPoliciesEntities())
                {
                    var alertInfo = alert.A_AlarmStrategy.Where(s => (s.DeviceID == deviceInfo.ID) && (s.DeviceItemId == itemInfo.ID) && (s.Active == true)).FirstOrDefault();
                    if (null != alertInfo)
                    {
                        if (isHitPolicies(curValue, alertInfo.Compare, alertInfo.Threshold))
                        {
                            log.InfoFormat("[MQTT]Hit Policies,topic:{0}/{1},curValue:{2},compare:{3},theshold:{4}.", deviceInfo.DeviceLabel, itemInfo.PropertyLabel, curValue, alertInfo.Compare, alertInfo.Threshold);
                            HistoryAlertPoliciesBLL historyBLL = new HistoryAlertPoliciesBLL();
                            HistoryAlertPoliciesModel model = new HistoryAlertPoliciesModel()
                            {
                                DeviceID = deviceInfo.ID.ToString(),
                                DeviceItemID = itemInfo.ID.ToString(),
                                StrategyID = alertInfo.ID.ToString(),
                                Value = curValue,
                                AlarmTime = now,
                                EndTime = now.AddSeconds(1),
                                OrgID = alertInfo.OrgID.ToString(),

                            };
                            historyBLL.AddHistoryAlertPolicies(model);
                            // 发送消息通知
                            UserApi api = new UserApi();
                            GetMessageInfoParameter messageModel = new GetMessageInfoParameter()
                            {
                                Type = "2", // 报警预警
                                Tittle = "【设备报警通知】" + deviceInfo.Name + itemInfo.Name + "报警！",
                                Text = "<p>您好：</p><p class='ql-indent-1'>" + deviceInfo.Name + "（设备）" + itemInfo.Name + "（属性）当前数值为" + curValue + "，已触发预设报警策略，请及时处理异常！</p><p><br></p><p class='ql-align-right'>设备在线监测平台</p>",
                                OrgID = alertInfo.OrgID.ToString()
                            };

                            var userApi = api.AddAlarmMessage(messageModel);
                            if (userApi.Code != 0)
                            {
                                log.ErrorFormat(userApi.Data);

                            }

                        }
                    }

                }
                return new Task<int>(() => result);
            }
            //异步处理报警逻辑(汉王数据接入使用)
            public async void AsyncHandleAlarmPolicies1(string deviceId, string air, string curValue, DateTime now)
            {
                RetDeviceInfo deviceInfo = new RetDeviceInfo();
                RetDeviceItemInfo itemInfo = new RetDeviceItemInfo();
                using (MonitoringEntities device = new MonitoringEntities())
                {
                    try
                    {
                        var searchDevice = device.D_Devices.Where(s => s.DeviceLabel == deviceId).FirstOrDefault();
                        if (searchDevice != null)
                        {
                            deviceInfo.ID = searchDevice.ID;
                            deviceInfo.Name = searchDevice.Name;
                            deviceInfo.DeviceLabel = searchDevice.DeviceLabel;
                            var searchDeviceItem = device.D_DevicesItem.Where(s => s.DeviceID == searchDevice.ID && s.PropertyLabel == air).FirstOrDefault();
                            if (searchDeviceItem != null)
                            {
                                itemInfo.ID = searchDeviceItem.ID;
                                itemInfo.Name = itemInfo.Name;
                                itemInfo.PropertyLabel = itemInfo.PropertyLabel;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    }
                    var result = await HandleAlarmPolicies1(deviceInfo, itemInfo, curValue, now);
                }
            }


            /// <summary>
            /// 判断是否命中报警策略
            /// </summary>
            /// <param name="curValue"></param>
            /// <param name="compare"></param>
            /// <param name="threshold"></param>
            /// <returns></returns>
            private bool isHitPolicies(string curValue, string compare, string threshold)
            {
                try
                {
                    decimal decValue = decimal.Parse(curValue);
                    decimal refValue = decimal.Parse(threshold);
                    switch (compare)
                    {
                        case "1"://>
                            return decValue > refValue;
                        case "2"://>=
                            return decValue >= refValue;
                        case "3"://=
                            return decValue == refValue;
                        case "4"://<
                            return decValue < refValue;
                        case "5"://<=
                            return decValue <= refValue;
                        case "6"://!=
                            return decValue != refValue;
                        default:
                            return false;
                    }
                }
                catch (Exception)
                {
                    log.ErrorFormat("[MQTT]Hit Policies Failed,curValue:{0},compare:{1},theshold:{2}.", curValue, compare, threshold);
                    return false;
                }
            }
        }
        /// 工具类，将时间戳转换成标准时间
        private static DateTime ConvertToTime(string timeStamp)
        {
            DateTime time = DateTime.Now;
            if (string.IsNullOrEmpty(timeStamp))
            {
                return time;
            }
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp + "0000");
                TimeSpan toNow = new TimeSpan(lTime);
                time = dtStart.Add(toNow);
            }
            catch (Exception ex)
            {
            }
            return time;
        }
    }
    public class GetSensorDataByApiBLL
    {
        private static readonly string hanwnagHost = "http://api.hwlantian.com";

        public static List<airData> getAirData(string phone, string password)
        {
            string cookie = getCookie(phone, password);
            List<deviceInfo> deviceList = getdeviceList(cookie);
            List<airData> airList = getDataBydeviceInfoList(deviceList, cookie);
            return airList;
        }

        //根据用户名和密码获取response响应头的Set-Cookie
        public static string getCookie(string phone, string password)
        {
            try
            { 
                String md5password = CommonTool.GetMd5(password);
                String parm = "Phone " + phone + ":" + md5password;
                //string parm = "Phone 15064113079:96b06e8a540f88dd3d686779fac2b8ae";
                string Url = hanwnagHost + "/user";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json; charset=UTF-8";
                request.Headers.Add("X-Authorization", parm);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string cookie = response.GetResponseHeader("Set-Cookie");
                if (cookie == null)
                {
                    throw new Exception("获取cookie失败");
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                return cookie;
            }
            catch
            {
                return null;
            }
        }

        //获取设备列表
        public static List<deviceInfo> getdeviceList(string cookie)
        {
            try
            {
                string Url = hanwnagHost + "/adapter";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json; charset=UTF-8";
                request.Headers.Add("cookie", cookie);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd().Trim();

                List<deviceInfo> deviceInfoList = JsonConvert.DeserializeObject<List<deviceInfo>>(retString);

                myStreamReader.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                deviceInfoList.Remove(deviceInfoList[0]);
                return deviceInfoList;
            }
            catch
            {
                return null;
            }
        }
        //根据设备列表获取每个设备最近上传的数据
        public static List<airData> getDataBydeviceInfoList(List<deviceInfo> deviceInfoList, string cookie)
        {
            try
            {
                List<airData> airDataList = new List<airData>();
                foreach (deviceInfo deviceInfo in deviceInfoList)
                {
                    if (!deviceInfo.linked.type.Equals("device"))
                    {

                        continue;
                    }
                    string Url = hanwnagHost + "/device/" + deviceInfo.linked.id + "/data/latest";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=UTF-8";
                    request.Headers.Add("cookie", cookie);

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string retString = myStreamReader.ReadToEnd().Trim();
                    airData ad = JsonConvert.DeserializeObject<airData>(retString);

                    myStreamReader.Close();

                    if (response != null)
                    {
                        response.Close();
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                    airDataList.Add(ad);
                }
                return airDataList;
            }
            catch
            {
                return null;
            }
        }

    }

    //下面3个类为json转c#类自动转过来的deviceList
    public class Linked
    {
        public string type { get; set; }
        public string id { get; set; }
    }
    public class Customize
    {
        public string name { get; set; }
        public string visitTime { get; set; }
    }
    public class deviceInfo
    {
        public string id { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string accountId { get; set; }
        public Linked linked { get; set; }
        public string createTime { get; set; }
        public Customize customize { get; set; }
    }

    //下面3个类为json转c#类自动转过来的airDataList
    public class Air
    {
        public string pm2d5 { get; set; }
        public string pm10 { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
        public string pm1d0 { get; set; }
        public string co2 { get; set; }
        public string tvoc { get; set; }
        public string ch2o { get; set; }
        public string rssi { get; set; }
        public string _interval { get; set; }
    }

    public class Data
    {
        public string timestamp { get; set; }
        public string recordCount { get; set; }
        public string duration { get; set; }
        public Air data { get; set; }
    }

    public class airData
    {
        public string deviceId { get; set; }
        public List<Data> data { get; set; }
    }
}
