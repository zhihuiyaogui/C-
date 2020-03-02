using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceMonitoringBLL.Model.Return.DeviceData;
using GenerSoft.OpenTSDB.Client;
using Newtonsoft.Json.Linq;
using DeviceMonitoringBLL.Model.Parameter.DeviceData;
using DeviceMonitoringDAL;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using GenerSoft.IndApp.CommonSdk;

namespace DeviceMonitoringBLL
{
    public class DeviceDataBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 示例
        /// </summary>
        public ReturnItem<RetDeviceTableData> GetData()
        {
            string opentsdburl = "http://172.22.15.132:10042";
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            List<string> UnixTime = new List<string>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();
            string resContent = "";
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("room", "room1");
                resContent = client.getData("nq_cu_heat", tagMap, OpentsdbClient.AGGREGATOR_AVG, "1d", "2013-12-10 12:00:00", "2014-01-01 13:00:00");
                //log.info(">>>" + resContent);
                resContent = resContent.Replace("[", string.Empty).Replace("]", string.Empty);
                // 时间戳和数据分开
                JObject obj = JObject.Parse(resContent);
                foreach (var x in obj)
                {
                    if (x.Key.ToString() == "dps")
                    {
                        JObject cha = JObject.Parse(x.Value.ToString());
                        foreach (var m in cha)
                        {
                            UnixTime.Add(m.Key.ToString());
                            Data.Add(Math.Round(Convert.ToDouble(m.Value), 2));
                        }
                    }
                }
                // Unix时间戳转换为C# DateTime
                foreach (var item in UnixTime)
                {
                    long unixTimeStamp = Convert.ToInt32(item);
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    DateTime dt = startTime.AddSeconds(unixTimeStamp);
                    Time.Add(dt.ToString());
                }

                var deviceinfo = new RetDeviceTableData();
                deviceinfo.Time = Time;
                deviceinfo.Data = Data;

                r.Msg = "设备数据获取成功";
                r.Code = 0;
                r.Data = deviceinfo;

                return r;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return r;
        }

        public void setAirRank(List<RetDeviceCurrentData> list)
        {
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    foreach(var info in list)
                    {
                        string mystr = info.DeviceItemId;
                        long mylong = long.Parse(mystr);
                        double air = double.Parse(info.Value);//将空气数值转int型
                        var deviceinfo1 = device.D_DevicesItem.Where(x => x.ID == mylong).FirstOrDefault();

                        if (deviceinfo1.PropertyLabel == "ch2o")
                        {
                            if (air <= 0.08)
                            {
                                info.airRank = "理想";
                            }
                            else if (air > 0.08 && air <= 0.1)
                            {
                                info.airRank = "一般";
                            }
                            else if (air > 0.1 && air <= 0.5)
                            {
                                info.airRank = "有害";
                            }
                            else if (air > 0.5)
                            {
                                info.airRank = "严重";
                            }

                        }
                        else if (deviceinfo1.PropertyLabel == "co2")
                        {
                            if (air <= 800)
                            {
                                info.airRank = "理想";
                            }
                            else if (air > 800 && air < 1700)
                            {
                                info.airRank = "正常";
                            }
                            else if (air >= 1700 && air < 3000)
                            {
                                info.airRank = "浑浊";
                            }
                            else if (air >= 3000 && air < 5500)
                            {
                                info.airRank = "中度污染";
                            }
                            else if (air > 5500)
                            {
                                info.airRank = "重度污染";
                            }

                        }
                        else if (deviceinfo1.PropertyLabel == "pm1d0")
                        {
                            double rate = 0.52;
                            if (air <= 35 * rate)
                            {
                                info.airRank = "优";
                            }
                            else if (air > 35 * rate && air <= 75 * rate)
                            {
                                info.airRank = "良";
                            }
                            else if (air > 75 * rate && air <= 115 * rate)
                            {
                                info.airRank = "轻度污染";
                            }
                            else if (air > 115 * rate && air <= 150 * rate)
                            {
                                info.airRank = "中度污染";
                            }
                            else if (air > 150 * rate && air <= 250 * rate)
                            {
                                info.airRank = "重度污染";
                            }
                            else if (air > 250 * rate)
                            {
                                info.airRank = "严重污染";
                            }
                        }
                        else if (deviceinfo1.PropertyLabel == "pm2d5")
                        {
                            if (air <= 35)
                            {
                                info.airRank = "优";
                            }
                            else if (air > 35 && air <= 75)
                            {
                                info.airRank = "良";
                            }
                            else if (air > 75 && air <= 115)
                            {
                                info.airRank = "轻度污染";
                            }
                            else if (air > 115 && air <= 150)
                            {
                                info.airRank = "中度污染";
                            }
                            else if (air > 150 && air <= 250)
                            {
                                info.airRank = "重度污染";
                            }
                            else if (air > 250)
                            {
                                info.airRank = "严重污染";
                            }
                        }
                        else if (deviceinfo1.PropertyLabel == "temperature")
                        {
                            if (air <= 4)
                            {
                                info.airRank = "寒冷";
                            }
                            else if (air > 4 && air <= 18)
                            {
                                info.airRank = "冷";
                            }
                            else if (air > 18 && air <= 28)
                            {
                                info.airRank = "舒适";
                            }
                            else if (air > 28 && air <= 37)
                            {
                                info.airRank = "热";
                            }
                            else if (air > 37)
                            {
                                info.airRank = "酷热";
                            }
                        }
                        else if (deviceinfo1.PropertyLabel == "pm10")
                        {
                            if (air <= 50)
                            {
                                info.airRank = "优";
                            }
                            else if (air > 50 && air <= 150)
                            {
                                info.airRank = "良";
                            }
                            else if (air > 150 && air <= 250)
                            {
                                info.airRank = "轻度污染";
                            }
                            else if (air > 250 && air <= 350)
                            {
                                info.airRank = "中度污染";
                            }
                            else if (air > 350 && air <= 420)
                            {
                                info.airRank = "重度污染";
                            }
                            else if (air > 420)
                            {
                                info.airRank = "严重污染";
                            }
                        }
                        else if (deviceinfo1.PropertyLabel == "humidity")
                        {
                            if (air <= 20)
                            {
                                info.airRank = "干燥";
                            }
                            else if (air > 20 && air <= 40)
                            {
                                info.airRank = "略干";
                            }
                            else if (air > 40 && air <= 65)
                            {
                                info.airRank = "舒适";
                            }
                            else if (air > 65)
                            {
                                info.airRank = "潮湿";
                            }

                        }
                        else if (deviceinfo1.PropertyLabel == "tvoc")
                        {
                            if (air <= 0.20)
                            {
                                info.airRank = "理想";
                            }
                            else if (air > 0.20 && air <= 0.40)
                            {
                                info.airRank = "良";
                            }
                            else if (air > 0.40 && air <= 0.80)
                            {
                                info.airRank = "良";
                            }
                            else if (air > 0.80)
                            {
                                info.airRank = "轻度污染";
                            }
                        }
                        else
                        {
                            info.airRank = "未知的item";
                        }
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                }
            }
        }
        /// <summary>
        /// 获取设备相关数据
        /// </summary>
        public RetDeviceRequiredData GetEquipmentData(DeviceRequiredDataModel model)
        {
            RetDeviceRequiredData data = new RetDeviceRequiredData();
            //获取设备名、设备标识符、TagMap、属性名、属性标识符、数据库（数据库、连接地址、用户名、密码）
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceid = Convert.ToInt32(model.DeviceID);
                    var deviceInfo = device.D_Devices.Where(s => s.ID == deviceid).FirstOrDefault();
                    if (deviceInfo == null)
                    {
                        return null;
                    }
                    if (deviceInfo != null)
                    {
                        data.DeviceName = deviceInfo.Name;
                        data.DeviceLabel = deviceInfo.DeviceLabel;
                        data.TagMap = deviceInfo.TagMap;
                        data.DataConnectID = deviceInfo.DataConnectID.ToString();

                        var deviceitemid = Convert.ToInt32(model.DeviceItemID);
                        var deviceitem = device.D_DevicesItem.Where(a => a.ID == deviceitemid).FirstOrDefault();
                        if (deviceitem == null)
                        {
                            return null;
                        }
                        if (deviceitem != null)
                        {
                            data.DeviceItemName = deviceitem.Name;
                            data.DeviceItemPropertyLabel = deviceitem.PropertyLabel;
                            if (deviceitem.Unit != null && deviceitem.Unit != "")
                            {
                                List<string> unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                                data.Unit = unit[1];
                            }
                        }

                        if (data.DataConnectID != null && !"".Equals(data.DataConnectID))
                        {
                            var dataconnectid = Convert.ToInt32(data.DataConnectID);
                            var getdatabase = device.D_DataConnectConfiguration.Where(s => s.ID == dataconnectid).FirstOrDefault();
                            if (getdatabase == null)
                            {
                                return null;
                            }
                            if (getdatabase != null)
                            {
                                data.DataConnectType = getdatabase.Type.ToString();
                                data.DataConnectServerAddress = getdatabase.ServerAddress;
                                data.DataConnectServerPort = getdatabase.ServerPort;
                                data.DataConnectUserName = getdatabase.UserName;
                                data.DataConnectPassWord = getdatabase.PassWord;
                                data.DataConnectDataBase = getdatabase.DataBase;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return data;
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// 通过设备ID获取设备及所有设备属性内容
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<RetDeviceRequiredData> GetAllEquipmentDataByDID(string ID)
        {
            List<RetDeviceRequiredData> dataList = new List<RetDeviceRequiredData>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceID = Convert.ToInt32(ID);
                    var deviceInfo = device.D_Devices.Where(s => s.ID == deviceID).FirstOrDefault();
                    var deviceItemInfo = device.D_DevicesItem.Where(a => a.DeviceID == deviceID).ToList();
                    if (deviceInfo == null)
                    {
                        return null;
                    }
                    if (deviceInfo != null)
                    {
                        if (deviceItemInfo == null)
                        {
                            return null;
                        }
                        else
                        {
                            foreach (var item in deviceItemInfo)
                            {
                                RetDeviceRequiredData data = new RetDeviceRequiredData();
                                data.DeviceName = deviceInfo.Name;
                                data.DeviceLabel = deviceInfo.DeviceLabel;
                                data.TagMap = deviceInfo.TagMap;
                                data.DataConnectID = deviceInfo.DataConnectID.ToString();
                                data.DeviceItemName = item.Name;
                                data.DeviceItemPropertyLabel = item.PropertyLabel;
                                if (item.Unit != null && item.Unit != "")
                                {
                                    List<string> unit = JsonConvert.DeserializeObject<List<string>>(item.Unit);
                                    data.Unit = unit[1];
                                }
                                if (data.DataConnectID != null && !"".Equals(data.DataConnectID))
                                {
                                    var dataconnectid = Convert.ToInt32(data.DataConnectID);
                                    var getdatabase = device.D_DataConnectConfiguration.Where(k => k.ID == dataconnectid).FirstOrDefault();
                                    if (getdatabase == null)
                                    {
                                        return null;
                                    }
                                    if (getdatabase != null)
                                    {
                                        data.DataConnectType = getdatabase.Type.ToString();
                                        data.DataConnectServerAddress = getdatabase.ServerAddress;
                                        data.DataConnectServerPort = getdatabase.ServerPort;
                                        data.DataConnectUserName = getdatabase.UserName;
                                        data.DataConnectPassWord = getdatabase.PassWord;
                                        data.DataConnectDataBase = getdatabase.DataBase;
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                                dataList.Add(data);
                            }
                        }
                    }
                    return dataList;
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// OpenTSDB获取数据
        /// </summary>
        public RetDeviceData OpenTSDBGetData(OpenTSDBRequiredDataModel model)
        {
            RetDeviceData deviceinfo = new RetDeviceData();
            List<RetDeviceTableList> TableList = new List<RetDeviceTableList>();
            List<RetPieList> PieList = new List<RetPieList>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();

            OpentsdbClient client;
            if (model.IsInspur)
            {
                client = new OpentsdbClient(model.url, model.UserName, model.PassWord, model.DataConnectID);
            }
            else
            {
                client = new OpentsdbClient(model.url);
            }
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                List<DeviceTagModel> taglist = new List<DeviceTagModel>();
                if (model.TagMap != "")
                {
                    taglist = JsonConvert.DeserializeObject<List<DeviceTagModel>>(model.TagMap);
                }
                foreach (var x in taglist)
                {
                    tagMap.Add(x.Key, x.Value);
                }
                string resContent = "";
                if (model.GetTogetherType == "1")
                {
                    resContent = client.getData(model.DeviceItemPropertyLabel, tagMap, OpentsdbClient.AGGREGATOR_AVG, model.Interval, model.StartTime, model.EndTime);
                }
                else if (model.GetTogetherType == "2")
                {
                    resContent = client.getData(model.DeviceItemPropertyLabel, tagMap, OpentsdbClient.AGGREGATOR_SUM, model.Interval, model.StartTime, model.EndTime);
                }
                else
                {
                    resContent = client.getData(model.DeviceItemPropertyLabel, tagMap, OpentsdbClient.AGGREGATOR_AVG, model.Interval, model.StartTime, model.EndTime);
                }
                if (string.IsNullOrEmpty(resContent))
                {
                    return null;
                }
                resContent = resContent.Replace("[", string.Empty).Replace("]", string.Empty);
                if (resContent != null && !"".Equals(resContent))
                {
                    // 数据处理，时间戳和数据分开
                    JObject obj = JObject.Parse(resContent);
                    foreach (var x in obj)
                    {
                        if (x.Key.ToString() == "dps")
                        {
                            JObject cha = JObject.Parse(x.Value.ToString());
                            foreach (var m in cha)
                            {
                                // Unix时间戳转换为C# DateTime
                                long unixTimeStamp = Convert.ToInt32(m.Key.ToString());
                                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                                DateTime dt = startTime.AddSeconds(unixTimeStamp);
                                Time.Add(dt.ToString());
                                Data.Add(Math.Round(Convert.ToDouble(m.Value), 2));

                                var devicelist = new RetPieList();
                                devicelist.value = m.Value.ToString();
                                devicelist.name = dt.ToString();
                                PieList.Add(devicelist);

                                var devicetablelist = new RetDeviceTableList();
                                devicetablelist.DeviceName = model.DeviceName;
                                devicetablelist.DeviceItemName = model.DeviceItemName;
                                devicetablelist.Data = Math.Round(Convert.ToDouble(m.Value), 2).ToString();
                                devicetablelist.Time = dt;
                                TableList.Add(devicetablelist);
                            }
                        }
                    }
                    deviceinfo.Time = Time;
                    deviceinfo.Data = Data;
                    deviceinfo.PieList = PieList;
                    deviceinfo.DeviceTableList = TableList;
                    deviceinfo.DeviceTableList.Reverse();
                    deviceinfo.DeviceItemName = model.DeviceItemName;
                    return deviceinfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// OpenTSDB获取最新数据
        /// </summary>
        public RetDeviceData OpenTSDBGetRecentData(OpenTSDBRequiredDataModel model)
        {
            RetDeviceData deviceinfo = new RetDeviceData();
            List<RetDeviceTableList> TableList = new List<RetDeviceTableList>();
            List<RetPieList> PieList = new List<RetPieList>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();

            OpentsdbClient client;
            if (model.IsInspur)
            {
                client = new OpentsdbClient(model.url, model.UserName, model.PassWord, model.DataConnectID);
            }
            else
            {
                client = new OpentsdbClient(model.url);
            }
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                List<DeviceTagModel> taglist = new List<DeviceTagModel>();
                if (model.TagMap != "")
                {
                    taglist = JsonConvert.DeserializeObject<List<DeviceTagModel>>(model.TagMap);
                }
                foreach (var x in taglist)
                {
                    tagMap.Add(x.Key, x.Value);
                }
                GenerSoft.OpenTSDB.Client.opentsdb.client.response.QueryLastResponse res = client.queryLastData(model.DeviceItemPropertyLabel, tagMap);
                if (res != null)
                {
                    long unixTimeStamp = Convert.ToInt32(res.timestamp.Substring(0, 10));
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    DateTime dt = startTime.AddSeconds(unixTimeStamp);
                    Time.Add(dt.ToString());
                    Data.Add(Math.Round(Convert.ToDouble(res.value), 2));

                    var devicelist = new RetPieList();
                    devicelist.value = res.value;
                    devicelist.name = dt.ToString();
                    PieList.Add(devicelist);

                    var devicetablelist = new RetDeviceTableList();
                    devicetablelist.DeviceName = model.DeviceName;
                    devicetablelist.DeviceItemName = model.DeviceItemName;
                    devicetablelist.Data = Math.Round(Convert.ToDouble(res.value), 2).ToString();
                    devicetablelist.Time = dt;
                    TableList.Add(devicetablelist);

                    deviceinfo.Time = Time;
                    deviceinfo.Data = Data;
                    deviceinfo.PieList = PieList;
                    deviceinfo.DeviceTableList = TableList;
                    deviceinfo.DeviceTableList.Reverse();
                    deviceinfo.DeviceItemName = model.DeviceItemName;
                    return deviceinfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// MySql获取数据
        /// </summary>
        public RetDeviceData MySqlGetData(MySqlRequiredDataModel model)
        {
            RetDeviceData deviceinfo = new RetDeviceData();
            List<RetDeviceTableList> TableList = new List<RetDeviceTableList>();
            List<RetPieList> PieList = new List<RetPieList>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();

            // 数据库配置
            string connStr = "Database=" + model.DataConnectDataBase + ";datasource=" + model.DataConnectServerAddress + ";port=" + model.DataConnectServerPort + ";user=" + model.DataConnectUserName + ";pwd=" + model.DataConnectPassWord + ";SslMode = none;";
            MySqlConnection conn = new MySqlConnection(connStr);

            //设置查询命令
            MySqlCommand cmd = new MySqlCommand("select * from " + model.DeviceLabel, conn);
            //查询结果读取器
            MySqlDataReader reader = null;

            try
            {
                //打开连接
                conn.Open();
                //执行查询，并将结果返回给读取器
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string data = reader.GetString(model.DeviceItemPropertyLabel);
                    string time = reader.GetString("CreateTime");
                    Data.Add(Math.Round(Convert.ToDouble(data), 2));
                    Time.Add(time);

                    var pielist = new RetPieList();
                    pielist.value = data;
                    pielist.name = time;
                    PieList.Add(pielist);

                    var tablelist = new RetDeviceTableList();
                    tablelist.DeviceName = model.DeviceName;
                    tablelist.DeviceItemName = model.DeviceItemName;
                    tablelist.Data = Math.Round(Convert.ToDouble(data), 2).ToString();
                    tablelist.Time = Convert.ToDateTime(time);
                    TableList.Add(tablelist);
                }

                deviceinfo.Time = Time;
                deviceinfo.Data = Data;
                deviceinfo.PieList = PieList;
                deviceinfo.DeviceTableList = TableList;
                deviceinfo.DeviceTableList.Reverse();
                deviceinfo.DeviceItemName = model.DeviceItemName;

                return deviceinfo;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// MySql获取一段时间内数据
        /// </summary>
        public RetDeviceData MySqlGetPeriodData(MySqlRequiredDataModel model)
        {
            RetDeviceData deviceinfo = new RetDeviceData();
            List<RetDeviceTableList> TableList = new List<RetDeviceTableList>();
            List<RetPieList> PieList = new List<RetPieList>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();

            // 数据库配置
            string connStr = "Database=" + model.DataConnectDataBase + ";datasource=" + model.DataConnectServerAddress + ";port=" + model.DataConnectServerPort + ";user=" + model.DataConnectUserName + ";pwd=" + model.DataConnectPassWord + ";SslMode = none;";
            MySqlConnection conn = new MySqlConnection(connStr);

            //设置查询命令
            MySqlCommand cmd = new MySqlCommand("select * from " + model.DeviceLabel + " where '" + model.StartTime + "' <= CreateTime and CreateTime <= '" + model.EndTime + "'", conn);
            //查询结果读取器
            MySqlDataReader reader = null;

            try
            {
                //打开连接
                conn.Open();
                //执行查询，并将结果返回给读取器
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string data = reader.GetString(model.DeviceItemPropertyLabel);
                    string time = reader.GetString("CreateTime");
                    Data.Add(Math.Round(Convert.ToDouble(data), 2));
                    Time.Add(time);

                    var pielist = new RetPieList();
                    pielist.value = data;
                    pielist.name = time;
                    PieList.Add(pielist);

                    var tablelist = new RetDeviceTableList();
                    tablelist.DeviceName = model.DeviceName;
                    tablelist.DeviceItemName = model.DeviceItemName;
                    tablelist.Data = Math.Round(Convert.ToDouble(data), 2).ToString();
                    tablelist.Time = Convert.ToDateTime(time);
                    TableList.Add(tablelist);
                }

                deviceinfo.Time = Time;
                deviceinfo.Data = Data;
                deviceinfo.PieList = PieList;
                deviceinfo.DeviceTableList = TableList;
                deviceinfo.DeviceTableList.Reverse();
                deviceinfo.DeviceItemName = model.DeviceItemName;

                return deviceinfo;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// MySql获取最新数据
        /// </summary>
        public RetDeviceData MySqlGetRecentData(MySqlRequiredDataModel model)
        {
            RetDeviceData deviceinfo = new RetDeviceData();
            List<RetDeviceTableList> TableList = new List<RetDeviceTableList>();
            List<RetPieList> PieList = new List<RetPieList>();
            List<string> Time = new List<string>();
            List<double> Data = new List<double>();
            List<RetDeviceTableList> AllTableList = new List<RetDeviceTableList>();
            List<RetPieList> AllPieList = new List<RetPieList>();
            List<string> AllTime = new List<string>();
            List<double> AllData = new List<double>();

            // 数据库配置
            string connStr = "Database=" + model.DataConnectDataBase + ";datasource=" + model.DataConnectServerAddress + ";port=" + model.DataConnectServerPort + ";user=" + model.DataConnectUserName + ";pwd=" + model.DataConnectPassWord + ";SslMode = none;";
            MySqlConnection conn = new MySqlConnection(connStr);

            //设置查询命令
            MySqlCommand cmd = new MySqlCommand("select * from " + model.DeviceLabel, conn);
            //查询结果读取器
            MySqlDataReader reader = null;

            try
            {
                //打开连接
                conn.Open();
                //执行查询，并将结果返回给读取器
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string data = reader.GetString(model.DeviceItemPropertyLabel);
                    string time = reader.GetString("CreateTime");
                    AllData.Add(Math.Round(Convert.ToDouble(data), 2));
                    AllTime.Add(time);

                    var pielist = new RetPieList();
                    pielist.value = data;
                    pielist.name = time;
                    AllPieList.Add(pielist);

                    var tablelist = new RetDeviceTableList();
                    tablelist.DeviceName = model.DeviceName;
                    tablelist.DeviceItemName = model.DeviceItemName;
                    tablelist.Data = Math.Round(Convert.ToDouble(data), 2).ToString();
                    tablelist.Time = Convert.ToDateTime(time);
                    AllTableList.Add(tablelist);
                }
                Time.Add(AllTime[AllTime.Count - 1]);
                Data.Add(AllData[AllData.Count - 1]);
                PieList.Add(AllPieList[AllPieList.Count - 1]);
                TableList.Add(AllTableList[AllTableList.Count - 1]);

                deviceinfo.Time = Time;
                deviceinfo.Data = Data;
                deviceinfo.PieList = PieList;
                deviceinfo.DeviceTableList = TableList;
                deviceinfo.DeviceTableList.Reverse();
                deviceinfo.DeviceItemName = model.DeviceItemName;

                return deviceinfo;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// 获取实时数据信息
        /// </summary>
        public ReturnItem<List<RetLastData>> GetLastData(ResponseLastData model)
        {
            ReturnItem<List<RetLastData>> r = new ReturnItem<List<RetLastData>>();
            List<RetLastData> RLDs = new List<RetLastData>();
            DeviceRequiredDataModel device = new DeviceRequiredDataModel();
            var ID = model.ID;
            List<RetDeviceRequiredData> DeviceData = GetAllEquipmentDataByDID(ID);
            //获取TSDB数据
            if (DeviceData == null)
            {
                return null;
            }
            else
            {
                foreach (var item in DeviceData)
                {
                    RetLastData RLD = new RetLastData();
                    if (item.DataConnectType == "4" || item.DataConnectType == "5" && item.TagMap != "" &&
                        item.DeviceItemPropertyLabel != "" && item.DataConnectServerAddress != "" &&
                        item.DataConnectServerPort != "")
                    {
                        var deviceItemPropertyLabel = item.DeviceItemPropertyLabel;
                        var url = "http://" + item.DataConnectServerAddress + ":" + item.DataConnectServerPort;
                        var UserName = item.DataConnectUserName;
                        var PassWord = item.DataConnectPassWord;
                        var DataConnectID = item.DataConnectID;
                        var isInspur = item.DataConnectType == "5" ? true : false;
                        OpentsdbClient client;
                        if (isInspur)
                        {
                            client = new OpentsdbClient(url, UserName, PassWord, DataConnectID);
                        }
                        else
                        {
                            client = new OpentsdbClient(url);
                        }
                        try
                        {
                            Dictionary<string, string> tagMap = new Dictionary<string, string>();
                            List<DeviceTagModel> taglist = new List<DeviceTagModel>();
                            if (item.TagMap != "")
                            {
                                taglist = JsonConvert.DeserializeObject<List<DeviceTagModel>>(item.TagMap);
                            }
                            foreach (var x in taglist)
                            {
                                tagMap.Add(x.Key, x.Value);
                            }
                            try
                            {
                                GenerSoft.OpenTSDB.Client.opentsdb.client.response.QueryLastResponse res =
                                client.queryLastData(deviceItemPropertyLabel, tagMap);
                                if (res != null)
                                {
                                    RLD.value = res.value;
                                }
                                else
                                {
                                    RLD.value = "暂无数据";
                                }
                            }
                            catch (Exception e)
                            {
                                log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                                RLD.value = "暂无数据";
                            }
                            RLD.deviceItemPropertyLabel = item.DeviceItemPropertyLabel;
                            RLDs.Add(RLD);
                        }
                        catch (Exception e)
                        {
                            log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                            continue;
                        }
                    }
                    //获取MySQL数据
                    else if (item.DataConnectType == "2" && item.DeviceLabel != "" && item.DeviceItemPropertyLabel != "" &&
                        item.DataConnectServerAddress != "" && item.DataConnectServerPort != ""
                        && item.DataConnectUserName != "" && item.DataConnectPassWord != "" &&
                        item.DataConnectDataBase != "")
                    {
                        // 数据库配置
                        string connStr = "Database=" + item.DataConnectDataBase + ";datasource=" +
                            item.DataConnectServerAddress + ";port=" +
                            item.DataConnectServerPort + ";user=" + item.DataConnectUserName + ";pwd=" +
                            item.DataConnectPassWord + ";SslMode = none;";
                        MySqlConnection conn = new MySqlConnection(connStr);

                        //设置查询命令
                        MySqlCommand cmd = new MySqlCommand("select * from " + item.DeviceLabel, conn);
                        //查询结果读取器
                        MySqlDataReader reader = null;
                        try
                        {
                            //打开连接
                            conn.Open();
                            //执行查询，并将结果返回给读取器
                            reader = cmd.ExecuteReader();
                            string data = null;
                            while (reader.Read())
                            {
                                data = reader.GetString(item.DeviceItemPropertyLabel);
                            }
                            if (data != null)
                            {
                                RLD.value = data;
                            }
                            else
                            {
                                RLD.value = "暂无数据";
                            }
                            RLD.deviceItemPropertyLabel = item.DeviceItemPropertyLabel;
                            RLDs.Add(RLD);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                        }
                        finally
                        {
                            if (reader != null)
                            {
                                reader.Close();
                            }
                            conn.Close();
                        }
                    }
                    else
                    {
                        RLD.value = "暂无数据";
                        RLD.deviceItemPropertyLabel = item.DeviceItemPropertyLabel;
                        RLDs.Add(RLD);
                    }
                }
            }
            if (RLDs.Count() != 0)
            {
                r.Msg = "设备数据获取成功";
                r.Code = 0;
                r.Data = RLDs;
            }
            else
            {
                r.Msg = "未获取到设备数据";
                r.Code = -1;
                r.Data = null;
            }
            return r;
        }

        /// <summary>
        /// 获取设备图表数据信息
        /// </summary>
        public ReturnItem<RetDeviceChartData> GetEquipmentChartData(DeviceChartDataModel model)
        {
            ReturnItem<RetDeviceChartData> r = new ReturnItem<RetDeviceChartData>();
            if (model.DatabaseType == "0")
            {
                DeviceRequiredDataModel device = new DeviceRequiredDataModel();
                device.DeviceID = model.DeviceID;
                device.DeviceItemID = model.DeviceItemID;
                RetDeviceRequiredData DeviceData = GetEquipmentData(device);
                // OpenTSDB获取数据
                if (DeviceData.DataConnectType == "4" || DeviceData.DataConnectType == "5" && DeviceData.TagMap != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "")
                {
                    OpenTSDBRequiredDataModel tsdbdata = new OpenTSDBRequiredDataModel();
                    tsdbdata.url = "http://" + DeviceData.DataConnectServerAddress + ":" + DeviceData.DataConnectServerPort;
                    tsdbdata.DeviceName = DeviceData.DeviceName;
                    tsdbdata.DeviceItemName = DeviceData.DeviceItemName;
                    tsdbdata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                    tsdbdata.TagMap = DeviceData.TagMap;
                    tsdbdata.IsInspur = DeviceData.DataConnectType == "5" ? true : false;
                    tsdbdata.UserName = DeviceData.DataConnectUserName;
                    tsdbdata.PassWord = DeviceData.DataConnectPassWord;
                    tsdbdata.DataConnectID = DeviceData.DataConnectID;

                    // 获取实时数据（仪表盘、液体图、温度计、指标卡）
                    if ((model.ChartType == "4" || model.ChartType == "5" || model.ChartType == "6" || model.ChartType == "7") && model.DataType == "3")
                    {
                        RetDeviceData deviceinfo = OpenTSDBGetRecentData(tsdbdata);
                        if (deviceinfo != null)
                        {
                            RetDeviceChartData chartinfo = new RetDeviceChartData();
                            chartinfo.Data = deviceinfo.Data;
                            chartinfo.Time = deviceinfo.Time;
                            chartinfo.PieList = deviceinfo.PieList;
                            chartinfo.Unit = DeviceData.Unit;
                            r.Msg = "设备数据获取成功";
                            r.Code = 0;
                            r.Data = chartinfo;
                        }
                        else
                        {
                            r.Msg = "未获取到设备数据";
                            r.Code = -1;
                            r.Data = null;
                        }
                        return r;
                    }
                    // 获取时间段数据
                    else
                    {
                        //折线图、柱形图、饼图
                        if (model.ChartType == "1" || model.ChartType == "2" || model.ChartType == "3")
                        {
                            if (model.DataType == "1")
                            {
                                DateTime d = DateTime.Now;
                                DateTime ad;
                                if (model.RecentUnit == "1")
                                {
                                    ad = d.AddSeconds(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "2")
                                {
                                    ad = d.AddMinutes(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "3")
                                {
                                    ad = d.AddHours(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "4")
                                {
                                    ad = d.AddDays(-Convert.ToInt32(model.RecentInterval));
                                }
                                else
                                {
                                    ad = d.AddDays(-Convert.ToInt32(model.RecentInterval));
                                }
                                tsdbdata.StartTime = Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                tsdbdata.EndTime = Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.DataType == "2")
                            {
                                DateTime d = DateTime.Now;
                                DateTime ad = d.AddDays(-7);
                                tsdbdata.StartTime = model.StartTime == null ? Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                                tsdbdata.EndTime = model.EndTime == null ? Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        // 仪表盘、液体图、温度计、指标卡
                        else if (model.ChartType == "4" || model.ChartType == "5" || model.ChartType == "6" || model.ChartType == "7")
                        {
                            tsdbdata.StartTime = Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                            if (model.IntervalUnit == "1")
                            {
                                tsdbdata.EndTime = Convert.ToDateTime(model.StartTime).AddSeconds(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "2")
                            {
                                tsdbdata.EndTime = Convert.ToDateTime(model.StartTime).AddMinutes(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "3")
                            {
                                tsdbdata.EndTime = Convert.ToDateTime(model.StartTime).AddHours(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "4")
                            {
                                tsdbdata.EndTime = Convert.ToDateTime(model.StartTime).AddDays(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                tsdbdata.EndTime = Convert.ToDateTime(model.StartTime).AddDays(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        string IntervalUnit = "";
                        if (model.IntervalUnit == "1")
                        {
                            IntervalUnit = "s";
                        }
                        else if (model.IntervalUnit == "2")
                        {
                            IntervalUnit = "m";
                        }
                        else if (model.IntervalUnit == "3")
                        {
                            IntervalUnit = "h";
                        }
                        else if (model.IntervalUnit == "4")
                        {
                            IntervalUnit = "d";
                        }
                        else
                        {
                            IntervalUnit = "h";
                        }
                        tsdbdata.Interval = model.StatisticalInterval + IntervalUnit;

                        RetDeviceData deviceinfo = OpenTSDBGetData(tsdbdata);
                        if (deviceinfo != null)
                        {
                            RetDeviceChartData chartinfo = new RetDeviceChartData();
                            chartinfo.DeviceName = DeviceData.DeviceName;
                            chartinfo.DeviceItemName = deviceinfo.DeviceItemName;
                            chartinfo.Data = deviceinfo.Data;
                            chartinfo.Time = deviceinfo.Time;
                            chartinfo.PieList = deviceinfo.PieList;
                            chartinfo.Unit = DeviceData.Unit;
                            r.Msg = "设备数据获取成功";
                            r.Code = 0;
                            r.Data = chartinfo;
                        }
                        else
                        {
                            r.Msg = "未获取到设备数据";
                            r.Code = -1;
                            r.Data = null;
                        }
                        return r;
                    }
                }
                // MySql获取数据
                else if (DeviceData.DataConnectType == "2" && DeviceData.DeviceLabel != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "" && DeviceData.DataConnectUserName != "" && DeviceData.DataConnectPassWord != "" && DeviceData.DataConnectDataBase != "")
                {
                    MySqlRequiredDataModel mysqldata = new MySqlRequiredDataModel();
                    mysqldata.DataConnectDataBase = DeviceData.DataConnectDataBase;
                    mysqldata.DataConnectServerAddress = DeviceData.DataConnectServerAddress;
                    mysqldata.DataConnectServerPort = DeviceData.DataConnectServerPort;
                    mysqldata.DataConnectUserName = DeviceData.DataConnectUserName;
                    mysqldata.DataConnectPassWord = DeviceData.DataConnectPassWord;
                    mysqldata.DeviceLabel = DeviceData.DeviceLabel;
                    mysqldata.DeviceName = DeviceData.DeviceName;
                    mysqldata.DeviceItemName = DeviceData.DeviceItemName;
                    mysqldata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;

                    RetDeviceData deviceinfo = new RetDeviceData();
                    // 获取实时数据（仪表盘、液体图、温度计、指标卡）
                    if ((model.ChartType == "4" || model.ChartType == "5" || model.ChartType == "6" || model.ChartType == "7") && model.DataType == "3")
                    {
                        deviceinfo = MySqlGetRecentData(mysqldata);
                    }
                    // 获取时间段数据
                    else
                    {
                        //折线图、柱形图、饼图
                        if (model.ChartType == "1" || model.ChartType == "2" || model.ChartType == "3")
                        {
                            if (model.DataType == "1")
                            {
                                DateTime d = DateTime.Now;
                                DateTime ad;
                                if (model.RecentUnit == "1")
                                {
                                    ad = d.AddSeconds(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "2")
                                {
                                    ad = d.AddMinutes(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "3")
                                {
                                    ad = d.AddHours(-Convert.ToInt32(model.RecentInterval));
                                }
                                else if (model.RecentUnit == "4")
                                {
                                    ad = d.AddDays(-Convert.ToInt32(model.RecentInterval));
                                }
                                else
                                {
                                    ad = d.AddDays(-Convert.ToInt32(model.RecentInterval));
                                }
                                mysqldata.StartTime = Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                mysqldata.EndTime = Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.DataType == "2")
                            {
                                DateTime d = DateTime.Now;
                                DateTime ad = d.AddDays(-7);
                                mysqldata.StartTime = model.StartTime == null ? Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                                mysqldata.EndTime = model.EndTime == null ? Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        // 仪表盘、液体图、温度计、指标卡
                        else if (model.ChartType == "4" || model.ChartType == "5" || model.ChartType == "6" || model.ChartType == "7")
                        {
                            mysqldata.StartTime = Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                            if (model.IntervalUnit == "1")
                            {
                                mysqldata.EndTime = Convert.ToDateTime(model.StartTime).AddSeconds(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "2")
                            {
                                mysqldata.EndTime = Convert.ToDateTime(model.StartTime).AddMinutes(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "3")
                            {
                                mysqldata.EndTime = Convert.ToDateTime(model.StartTime).AddHours(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (model.IntervalUnit == "4")
                            {
                                mysqldata.EndTime = Convert.ToDateTime(model.StartTime).AddDays(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                mysqldata.EndTime = Convert.ToDateTime(model.StartTime).AddDays(Convert.ToDouble(model.StatisticalInterval)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }

                        deviceinfo = MySqlGetPeriodData(mysqldata);
                    }

                    if (deviceinfo != null)
                    {
                        RetDeviceChartData chartinfo = new RetDeviceChartData();
                        chartinfo.DeviceName = DeviceData.DeviceName;
                        chartinfo.DeviceItemName = deviceinfo.DeviceItemName;
                        chartinfo.Data = deviceinfo.Data;
                        chartinfo.Time = deviceinfo.Time;
                        chartinfo.PieList = deviceinfo.PieList;
                        chartinfo.Unit = DeviceData.Unit;

                        r.Msg = "设备数据获取成功";
                        r.Code = 0;
                        r.Data = chartinfo;
                    }
                    else
                    {
                        r.Msg = "未获取到设备数据";
                        r.Code = -1;
                        r.Data = null;
                    }
                    return r;
                }
            }
            else if (model.DatabaseType == "1")
            {
                List<RetPieList> PieList = new List<RetPieList>();
                List<string> Time = new List<string>();
                List<double> Data = new List<double>();
                //设备总数
                if (model.ValueType == "2-1")
                {
                    using (MonitoringEntities device = new MonitoringEntities())
                    {
                        try
                        {
                            var getdevice = device.D_Devices.Where(s => s.OrgID == model.OrgID).ToList();
                            if (getdevice == null)
                            {
                                r.Data = null;
                                r.Code = -1;
                                r.Msg = "未找到设备";
                                return r;
                            }
                            if (getdevice != null)
                            {
                                Data.Add(getdevice.Count);
                                var deviceinfo = new RetDeviceChartData();
                                deviceinfo.Time = Time;
                                deviceinfo.Data = Data;
                                deviceinfo.PieList = PieList;
                                deviceinfo.Unit = "台";

                                r.Msg = "设备数据获取成功";
                                r.Code = 0;
                                r.Data = deviceinfo;
                            }
                        }
                        catch (Exception e)
                        {
                            r.Msg = "内部错误请重试";
                            log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                            r.Code = -1;
                        }
                    }
                }
                //设备位置
                else if (model.ValueType == "2-2")
                {
                    using (MonitoringEntities device = new MonitoringEntities())
                    {
                        try
                        {
                            Model.Parameter.DeviceMonitoring.DeviceInfoModel parameter = new Model.Parameter.DeviceMonitoring.DeviceInfoModel();
                            parameter.OrgID = model.OrgID.ToString();
                            DeviceInfoBLL list = new DeviceInfoBLL();
                            var get = list.GetEquipmentList(parameter);

                            var deviceinfo = new RetDeviceChartData();
                            deviceinfo.DeviceList = get.Data;

                            r.Data = deviceinfo;
                            r.Msg = "设备数据获取成功";
                            r.Code = 0;
                        }
                        catch (Exception e)
                        {
                            r.Msg = "内部错误请重试";
                            log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                            r.Code = -1;
                        }
                    }
                }
                //报警数量
                else if (model.ValueType == "2-3")
                {
                    try
                    {
                        AlertPoliciesApi alertApi = new AlertPoliciesApi();
                        var num = alertApi.GetAlertPoliciesNum();
                        Data.Add(Convert.ToInt32(num.Data));
                        var deviceinfo = new RetDeviceChartData();
                        deviceinfo.Time = Time;
                        deviceinfo.Data = Data;
                        deviceinfo.PieList = PieList;
                        deviceinfo.Unit = "条";

                        r.Msg = "设备数据获取成功";
                        r.Code = 0;
                        r.Data = deviceinfo;
                    }
                    catch (Exception e)
                    {
                        r.Msg = "内部错误请重试";
                        log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                        r.Code = -1;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// 获取设备运行报表折线图数据信息
        /// </summary>
        public ReturnItem<RetDeviceTableData> GetEquipmentReportListData(DeviceDataModel model)
        {
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            DeviceRequiredDataModel device = new DeviceRequiredDataModel();
            device.DeviceID = model.DeviceID;
            device.DeviceItemID = model.DeviceItemID;
            RetDeviceRequiredData DeviceData = GetEquipmentData(device);
            // OpenTSDB获取数据
            if (DeviceData.DataConnectType == "4" || DeviceData.DataConnectType == "5" && DeviceData.TagMap != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "")
            {
                OpenTSDBRequiredDataModel tsdbdata = new OpenTSDBRequiredDataModel();
                tsdbdata.url = "http://" + DeviceData.DataConnectServerAddress + ":" + DeviceData.DataConnectServerPort;
                tsdbdata.DeviceName = DeviceData.DeviceName;
                tsdbdata.DeviceItemName = DeviceData.DeviceItemName;
                tsdbdata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                tsdbdata.TagMap = DeviceData.TagMap;
                tsdbdata.IsInspur = DeviceData.DataConnectType == "5" ? true : false;
                tsdbdata.UserName = DeviceData.DataConnectUserName;
                tsdbdata.PassWord = DeviceData.DataConnectPassWord;
                tsdbdata.DataConnectID = DeviceData.DataConnectID;

                DateTime d = DateTime.Now;
                DateTime ad = d.AddDays(-7);
                tsdbdata.StartTime = model.StartTime == null ? Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                tsdbdata.EndTime = model.EndTime == null ? Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd HH:mm:ss");
                string IntervalUnit = "";
                if (model.IntervalUnit == "1")
                {
                    IntervalUnit = "s";
                }
                else if (model.IntervalUnit == "2")
                {
                    IntervalUnit = "m";
                }
                else if (model.IntervalUnit == "3")
                {
                    IntervalUnit = "h";
                }
                else if (model.IntervalUnit == "4")
                {
                    IntervalUnit = "d";
                }
                else
                {
                    IntervalUnit = "h";
                }
                tsdbdata.Interval = model.StatisticalInterval + IntervalUnit;

                RetDeviceData deviceinfo = OpenTSDBGetData(tsdbdata);
                if (deviceinfo != null)
                {
                    RetDeviceTableData tableinfo = new RetDeviceTableData();
                    tableinfo.Data = deviceinfo.Data;
                    tableinfo.Time = deviceinfo.Time;
                    tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                    tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                    tableinfo.Unit = DeviceData.Unit;
                    tableinfo.DeviceName = tsdbdata.DeviceName;
                    r.Count = tableinfo.DeviceTableList.Count();
                    r.Msg = "设备数据获取成功";
                    r.Code = 0;
                    r.Data = tableinfo;
                }
                else
                {
                    r.Msg = "未获取到设备数据";
                    r.Code = -1;
                    r.Data = null;
                }
                return r;
            }
            // MySql获取数据
            else if (DeviceData.DataConnectType == "2" && DeviceData.DeviceLabel != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "" && DeviceData.DataConnectUserName != "" && DeviceData.DataConnectPassWord != "" && DeviceData.DataConnectDataBase != "")
            {
                MySqlRequiredDataModel mysqldata = new MySqlRequiredDataModel();
                mysqldata.DataConnectDataBase = DeviceData.DataConnectDataBase;
                mysqldata.DataConnectServerAddress = DeviceData.DataConnectServerAddress;
                mysqldata.DataConnectServerPort = DeviceData.DataConnectServerPort;
                mysqldata.DataConnectUserName = DeviceData.DataConnectUserName;
                mysqldata.DataConnectPassWord = DeviceData.DataConnectPassWord;
                mysqldata.DeviceLabel = DeviceData.DeviceLabel;
                mysqldata.DeviceName = DeviceData.DeviceName;
                mysqldata.DeviceItemName = DeviceData.DeviceItemName;
                mysqldata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                mysqldata.StartTime = model.StartTime.ToString();
                mysqldata.EndTime = model.EndTime.ToString();

                RetDeviceData deviceinfo = MySqlGetPeriodData(mysqldata);

                if (deviceinfo != null)
                {
                    RetDeviceTableData tableinfo = new RetDeviceTableData();
                    tableinfo.Data = deviceinfo.Data;
                    tableinfo.Time = deviceinfo.Time;
                    tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                    tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                    tableinfo.Unit = DeviceData.Unit;
                    tableinfo.DeviceName = mysqldata.DeviceName;

                    r.Count = tableinfo.DeviceTableList.Count();
                    r.Msg = "设备数据获取成功";
                    r.Code = 0;
                    r.Data = tableinfo;
                }
                else
                {
                    r.Msg = "未获取到设备数据";
                    r.Code = -1;
                    r.Data = null;
                }
                return r;
            }
            return r;
        }

        /// <summary>
        /// 获取能源报表数据信息
        /// </summary>
        public ReturnItem<RetDeviceTableData> GetEnergyReportListData(DeviceDataModel model)
        {
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            DeviceRequiredDataModel device = new DeviceRequiredDataModel();
            device.DeviceID = model.DeviceID;
            device.DeviceItemID = model.DeviceItemID;
            RetDeviceRequiredData DeviceData = GetEquipmentData(device);
            // OpenTSDB获取数据
            if (DeviceData.DataConnectType == "4" || DeviceData.DataConnectType == "5" && DeviceData.TagMap != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "")
            {
                OpenTSDBRequiredDataModel tsdbdata = new OpenTSDBRequiredDataModel();
                tsdbdata.url = "http://" + DeviceData.DataConnectServerAddress + ":" + DeviceData.DataConnectServerPort;
                tsdbdata.DeviceName = DeviceData.DeviceName;
                tsdbdata.DeviceItemName = DeviceData.DeviceItemName;
                tsdbdata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                tsdbdata.TagMap = DeviceData.TagMap;
                tsdbdata.IsInspur = DeviceData.DataConnectType == "5" ? true : false;
                tsdbdata.UserName = DeviceData.DataConnectUserName;
                tsdbdata.PassWord = DeviceData.DataConnectPassWord;
                tsdbdata.DataConnectID = DeviceData.DataConnectID;

                DateTime d = DateTime.Now;
                DateTime ad = d.AddDays(-7);
                tsdbdata.StartTime = model.StartTime == null ? Convert.ToDateTime(ad.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
                tsdbdata.EndTime = model.EndTime == null ? Convert.ToDateTime(d.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd HH:mm:ss");
                string IntervalUnit = "";
                if (model.IntervalUnit == "1")
                {
                    IntervalUnit = "s";
                }
                else if (model.IntervalUnit == "2")
                {
                    IntervalUnit = "m";
                }
                else if (model.IntervalUnit == "3")
                {
                    IntervalUnit = "h";
                }
                else if (model.IntervalUnit == "4")
                {
                    IntervalUnit = "d";
                }
                else
                {
                    IntervalUnit = "h";
                }
                tsdbdata.Interval = model.StatisticalInterval + IntervalUnit;
                tsdbdata.GetTogetherType = model.GetTogetherType;

                RetDeviceData deviceinfo = OpenTSDBGetData(tsdbdata);
                if (deviceinfo != null)
                {
                    RetDeviceTableData tableinfo = new RetDeviceTableData();
                    tableinfo.Data = deviceinfo.Data;
                    tableinfo.Time = deviceinfo.Time;
                    tableinfo.DeviceName = DeviceData.DeviceName;
                    tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                    tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                    tableinfo.Unit = DeviceData.Unit;
                    r.Count = tableinfo.DeviceTableList.Count();
                    r.Msg = "设备数据获取成功";
                    r.Code = 0;
                    r.Data = tableinfo;
                }
                else
                {
                    r.Msg = "未获取到设备数据";
                    r.Code = -1;
                    r.Data = null;
                }
                return r;
            }
            // MySql获取数据
            else if (DeviceData.DataConnectType == "2" && DeviceData.DeviceLabel != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "" && DeviceData.DataConnectUserName != "" && DeviceData.DataConnectPassWord != "" && DeviceData.DataConnectDataBase != "")
            {
                MySqlRequiredDataModel mysqldata = new MySqlRequiredDataModel();
                mysqldata.DataConnectDataBase = DeviceData.DataConnectDataBase;
                mysqldata.DataConnectServerAddress = DeviceData.DataConnectServerAddress;
                mysqldata.DataConnectServerPort = DeviceData.DataConnectServerPort;
                mysqldata.DataConnectUserName = DeviceData.DataConnectUserName;
                mysqldata.DataConnectPassWord = DeviceData.DataConnectPassWord;
                mysqldata.DeviceLabel = DeviceData.DeviceLabel;
                mysqldata.DeviceName = DeviceData.DeviceName;
                mysqldata.DeviceItemName = DeviceData.DeviceItemName;
                mysqldata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                mysqldata.StartTime = model.StartTime.ToString();
                mysqldata.EndTime = model.EndTime.ToString();

                RetDeviceData deviceinfo = MySqlGetPeriodData(mysqldata);

                if (deviceinfo != null)
                {
                    RetDeviceTableData tableinfo = new RetDeviceTableData();
                    tableinfo.Data = deviceinfo.Data;
                    tableinfo.Time = deviceinfo.Time;
                    tableinfo.DeviceName = DeviceData.DeviceName;
                    tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                    tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                    tableinfo.Unit = DeviceData.Unit;

                    r.Count = tableinfo.DeviceTableList.Count();
                    r.Msg = "设备数据获取成功";
                    r.Code = 0;
                    r.Data = tableinfo;
                }
                else
                {
                    r.Msg = "未获取到设备数据";
                    r.Code = -1;
                    r.Data = null;
                }
                return r;
            }
            return r;
        }

        /// <summary>
        /// 根据设备id和设备属性id获取最新一条数据
        /// </summary>
        public ReturnItem<RetDeviceTableData> GetCurrentData(DeviceDataModel model)
        {
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            DeviceRequiredDataModel device = new DeviceRequiredDataModel();
            device.DeviceID = model.DeviceID;
            device.DeviceItemID = model.DeviceItemID;
            RetDeviceRequiredData DeviceData = GetEquipmentData(device);
            if (DeviceData != null) {
                // OpenTSDB获取数据
                if (DeviceData.DataConnectType == "4" || DeviceData.DataConnectType == "5" && DeviceData.TagMap != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "")
                {
                    OpenTSDBRequiredDataModel tsdbdata = new OpenTSDBRequiredDataModel();
                    tsdbdata.url = "http://" + DeviceData.DataConnectServerAddress + ":" + DeviceData.DataConnectServerPort;
                    tsdbdata.DeviceName = DeviceData.DeviceName;
                    tsdbdata.DeviceItemName = DeviceData.DeviceItemName;
                    tsdbdata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;
                    tsdbdata.TagMap = DeviceData.TagMap;
                    tsdbdata.IsInspur = DeviceData.DataConnectType == "5" ? true : false;
                    tsdbdata.UserName = DeviceData.DataConnectUserName;
                    tsdbdata.PassWord = DeviceData.DataConnectPassWord;
                    tsdbdata.DataConnectID = DeviceData.DataConnectID;

                    RetDeviceData deviceinfo = OpenTSDBGetRecentData(tsdbdata);
                    if (deviceinfo != null)
                    {
                        RetDeviceTableData tableinfo = new RetDeviceTableData();
                        tableinfo.Data = deviceinfo.Data;
                        tableinfo.Time = deviceinfo.Time;
                        tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                        tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                        tableinfo.Unit = DeviceData.Unit;
                        r.Count = tableinfo.DeviceTableList.Count();
                        r.Msg = "设备数据获取成功";
                        r.Code = 0;
                        r.Data = tableinfo;
                    }
                    else
                    {
                        r.Msg = "未获取到设备数据";
                        r.Code = -1;
                        r.Data = null;
                    }
                    return r;
                }
                // MySql获取数据
                else if (DeviceData.DataConnectType == "2" && DeviceData.DeviceLabel != "" && DeviceData.DeviceItemPropertyLabel != "" && DeviceData.DataConnectServerAddress != "" && DeviceData.DataConnectServerPort != "" && DeviceData.DataConnectUserName != "" && DeviceData.DataConnectPassWord != "" && DeviceData.DataConnectDataBase != "")
                {
                    MySqlRequiredDataModel mysqldata = new MySqlRequiredDataModel();
                    mysqldata.DataConnectDataBase = DeviceData.DataConnectDataBase;
                    mysqldata.DataConnectServerAddress = DeviceData.DataConnectServerAddress;
                    mysqldata.DataConnectServerPort = DeviceData.DataConnectServerPort;
                    mysqldata.DataConnectUserName = DeviceData.DataConnectUserName;
                    mysqldata.DataConnectPassWord = DeviceData.DataConnectPassWord;
                    mysqldata.DeviceLabel = DeviceData.DeviceLabel;
                    mysqldata.DeviceName = DeviceData.DeviceName;
                    mysqldata.DeviceItemName = DeviceData.DeviceItemName;
                    mysqldata.DeviceItemPropertyLabel = DeviceData.DeviceItemPropertyLabel;

                    RetDeviceData deviceinfo = MySqlGetRecentData(mysqldata);

                    if (deviceinfo != null)
                    {
                        RetDeviceTableData tableinfo = new RetDeviceTableData();
                        tableinfo.Data = deviceinfo.Data;
                        tableinfo.Time = deviceinfo.Time;
                        tableinfo.DeviceItemName = deviceinfo.DeviceItemName;
                        tableinfo.DeviceTableList = deviceinfo.DeviceTableList;
                        tableinfo.Unit = DeviceData.Unit;

                        r.Count = tableinfo.DeviceTableList.Count();
                        r.Msg = "设备数据获取成功";
                        r.Code = 0;
                        r.Data = tableinfo;
                    }
                    else
                    {
                        r.Msg = "未获取到设备数据";
                        r.Code = -1;
                        r.Data = null;
                    }
                    return r;
                }
            }
            return r;
        }

        /// <summary>
        /// 内部接口获取设备当前数据信息
        /// </summary>
        public ReturnItem<RetDeviceCurrentData> GetDeviceCurrentDataInside(DeviceDataInfoModel model)
        {
            ReturnItem<RetDeviceCurrentData> r = new ReturnItem<RetDeviceCurrentData>();
            var DeviceName = "";
            var DeviceLabel = "";
            var TagMap = model.TagMap;
            var DeviceItemId = "";
            var DeviceItemName = "";
            var DeviceItemPropertyLabel = "";
            var DataConnectID = model.DataConnectID;
            var DataConnectType = "";
            var DataConnectServerAddress = "";
            var DataConnectServerPort = "";
            var DataConnectUserName = "";
            var DataConnectPassWord = "";
            var DataConnectDataBase = "";
            var Unit = "";
            //获取设备名、属性名、数据库连接地址
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceid = Convert.ToInt32(model.DeviceID);
                    var deviceInfo = device.D_Devices.Where(s => s.ID == deviceid).FirstOrDefault();
                    if (deviceInfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备";
                        return r;
                    }
                    if (deviceInfo != null)
                    {
                        DeviceName = deviceInfo.Name;
                        DeviceLabel = deviceInfo.DeviceLabel;
                        var deviceitemid = Convert.ToInt32(model.DeviceItemID);
                        var deviceitem = device.D_DevicesItem.Where(a => a.ID == deviceitemid).FirstOrDefault();
                        if (deviceitem == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到该设备属性";
                            return r;
                        }
                        if (deviceitem != null)
                        {
                            DeviceItemName = deviceitem.Name;
                            DeviceItemId = deviceitem.ID.ToString();
                            DeviceItemPropertyLabel = deviceitem.PropertyLabel;
                            if (deviceitem.Unit != null && deviceitem.Unit != "")
                            {
                                List<string> unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                                Unit = unit[1];
                            }
                        }

                        if (DataConnectID != null && !"".Equals(DataConnectID))
                        {
                            var dataconnectid = Convert.ToInt32(DataConnectID);
                            var getdatabase = device.D_DataConnectConfiguration.Where(s => s.ID == dataconnectid).FirstOrDefault();
                            if (getdatabase == null)
                            {
                                r.Data = null;
                                r.Code = -1;
                                r.Msg = "未找到数据库配置信息";
                                return r;
                            }
                            if (getdatabase != null)
                            {
                                DataConnectType = getdatabase.Type.ToString();
                                DataConnectServerAddress = getdatabase.ServerAddress;
                                DataConnectServerPort = getdatabase.ServerPort;
                                DataConnectUserName = getdatabase.UserName;
                                DataConnectPassWord = getdatabase.PassWord;
                                DataConnectDataBase = getdatabase.DataBase;
                            }
                        }
                        else
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到数据库配置信息";
                            return r;
                        }
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            // 获取数据
            if (DataConnectType == "4" || DataConnectType == "5" && TagMap != "" && DeviceItemPropertyLabel != "" && DataConnectServerAddress != "" && DataConnectServerPort != "")
            {
                OpentsdbClient client;
                if (DataConnectType == "4")
                {
                    client = new OpentsdbClient("http://" + DataConnectServerAddress + ":" + DataConnectServerPort);
                }
                else
                {
                    client = new OpentsdbClient("http://" + DataConnectServerAddress + ":" + DataConnectServerPort, DataConnectUserName, DataConnectPassWord, DataConnectID);
                }

                try
                {
                    Dictionary<string, string> tagMap = new Dictionary<string, string>();
                    List<DeviceTagModel> taglist = new List<DeviceTagModel>();
                    if (TagMap != "")
                    {
                        taglist = JsonConvert.DeserializeObject<List<DeviceTagModel>>(TagMap);
                    }
                    foreach (var x in taglist)
                    {
                        tagMap.Add(x.Key, x.Value);
                    }
                    GenerSoft.OpenTSDB.Client.opentsdb.client.response.QueryLastResponse res = client.queryLastData(DeviceItemPropertyLabel, tagMap);
                    var deviceinfo = new RetDeviceCurrentData();
                    if (res != null)
                    {
                        if (res.value != "" && res.value != null)
                        {
                            deviceinfo.Value = res.value;
                        }
                    }
                    else
                    {
                        deviceinfo.Value = "暂无数据";
                    }
                    deviceinfo.ID = model.ID;
                    deviceinfo.DeviceItemId = DeviceItemId;
                    deviceinfo.DeviceItemName = DeviceItemName;
                    deviceinfo.Unit = Unit;

                    r.Msg = "数据获取成功";
                    r.Code = 0;
                    r.Data = deviceinfo;
                    return r;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            // MySql获取数据
            else if (DataConnectType == "2" && DeviceLabel != "" && DeviceItemPropertyLabel != "" && DataConnectServerAddress != "" && DataConnectServerPort != "" && DataConnectUserName != "" && DataConnectPassWord != "" && DataConnectDataBase != "")
            {
                List<double> Data = new List<double>();
                // 数据库配置
                string connStr = "Database=" + DataConnectDataBase + ";datasource=" + DataConnectServerAddress + ";port=" + DataConnectServerPort + ";user=" + DataConnectUserName + ";pwd=" + DataConnectPassWord + ";SslMode = none;";
                MySqlConnection conn = new MySqlConnection(connStr);

                //设置查询命令
                MySqlCommand cmd = new MySqlCommand("select * from " + DeviceLabel, conn);
                //查询结果读取器
                MySqlDataReader reader = null;

                try
                {
                    //打开连接
                    conn.Open();
                    //执行查询，并将结果返回给读取器
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string data = reader.GetString(DeviceItemPropertyLabel);
                        Data.Add(Math.Round(Convert.ToDouble(data), 2));
                    }

                    var deviceinfo = new RetDeviceCurrentData();
                    if (Data.Count > 0)
                    {
                        deviceinfo.Value = Data[Data.Count - 1].ToString();
                    }
                    else
                    {
                        deviceinfo.Value = "暂无数据";
                    }
                    deviceinfo.ID = model.ID;
                    deviceinfo.DeviceItemId = DeviceItemId;
                    deviceinfo.DeviceItemName = DeviceItemName;
                    deviceinfo.Unit = Unit;

                    r.Msg = "数据获取成功";
                    r.Code = 0;
                    r.Data = deviceinfo;
                    return r;

                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }
        /// <summary>
        /// 获得处理后的报表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReturnItem<RetDeviceTableData> GetEquipmentReportListDataByHandl(DeviceDataModel model)
        {
            ReturnItem<RetDeviceTableData> r = new ReturnItem<RetDeviceTableData>();
            ReturnItem<RetDeviceTableData> rr = new ReturnItem<RetDeviceTableData>();//接收GetEquipmentReportListData数据
            RetDeviceTableData deviceData = new RetDeviceTableData();
            rr = GetEquipmentReportListData(model);
            if (rr.Code >= 0)
            {
                deviceData = rr.Data;
                // OpenTSDB获取数据
                RetDeviceTableData deviceList = AlgorithmBLL.Gaussain(deviceData);
                if (deviceList != null)
                {
                    RetDeviceTableData tableinfo = new RetDeviceTableData();
                    tableinfo.Data = deviceList.Data;
                    tableinfo.Time = deviceList.Time;
                    tableinfo.DeviceItemName = deviceList.DeviceItemName;
                    tableinfo.DeviceTableList = deviceList.DeviceTableList;
                    tableinfo.Unit = deviceData.Unit;
                    //tableinfo.AllCoordination = deviceList.AllCoordination;
                    tableinfo.NormCoordination = deviceList.NormCoordination;
                    tableinfo.AnomCoordination = deviceList.AnomCoordination;
                    tableinfo.DeviceName = deviceData.DeviceName;
                    r.Count = tableinfo.DeviceTableList.Count();
                    r.Msg = "设备数据获取成功";
                    r.Code = 0;
                    r.Data = tableinfo;
                }
                else
                {
                    r.Msg = "未获取到设备数据";
                    r.Code = -1;
                    r.Data = null;
                }
                return r;
            }
            else
            {
                r.Msg = rr.Msg;
                r.Code = rr.Code;
                r.Data = rr.Data;
            }
            return r;
        }
    }
}
