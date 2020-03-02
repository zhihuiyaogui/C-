using Common;
using GenerSoft.IndApp.AlertPoliciesBLL.Model;
using GenerSoft.IndApp.AlertPoliciesBLL.Model.Parameter.HistoryAlertPolicies;
using GenerSoft.IndApp.AlertPoliciesDAL;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.IndApp.CommonSdk.Model.User;
using GenerSoft.MQTT.Client;
using GenerSoft.OpenTSDB.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    /// <summary>
    /// MQTT Service 容器
    /// 连接一个MQTT Server 则一个Service
    /// 对外提供根据connnectID获取Service的方法
    /// </summary>
    public class MqttServiceContainer

    {
        private static volatile MqttServiceContainer instance;

        private static readonly object obj = new object();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<long, MqttClientService> container = new Dictionary<long, MqttClientService>();

        private MqttServiceContainer() { }

        public static MqttServiceContainer Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (obj)
                    {
                        if (null == instance)
                        {
                            instance = new MqttServiceContainer();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 查询某连接是否存在
        /// </summary>
        /// <param name="connectID"></param>
        /// <returns></returns>
        public bool IsClientExist(long connectID)
        {
            return container.ContainsKey(connectID);
        }

        public MqttClientService GetMqttServiceByConnectID(long id)
        {
            /*
            * 查询容器中是否已经有客户端
            * 如果存在，检查该客户端连接是否生效。
            * 1.生效，返回该客户端 
            * 2.失效，移除该客户端，获取一个新的客户端，放到容器中，并返回客户端
            */
            if (container.ContainsKey(id))
            {
                MqttClientService service = container[id];
                if (service.mqttClient.IsConnected)
                {
                    return service;
                }
                else
                {
                    container.Remove(id);
                    return GetNewMqttServiceByConnectID(id);
                }

            }
            else
            {
                return GetNewMqttServiceByConnectID(id);
            }
        }

        /// <summary>
        /// 根据物接入ID获取一个新的client serivce
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private MqttClientService GetNewMqttServiceByConnectID(long id)
        {
            DeviceMonitoringApi deviceApi = new DeviceMonitoringApi();
            IoTHubConfigurationModel model = new IoTHubConfigurationModel();
            model.ID = id;
            var conf = deviceApi.GetIoTHubConnection(model);
            if (conf.Code == -1)
            {
                throw new Exception("获取IoTHub连接信息失败" + conf.Msg);
            }
            RetIoTHubConfiguration connectInfo = (RetIoTHubConfiguration)conf.Data;

            string[] ipPort = connectInfo.Url.Replace("http://", "").Replace("HTTP://", "").Split(':');
            string ip = ipPort[0];
            int port;
            try
            {
                port = ipPort.Count() > 1 ? Int32.Parse(ipPort[1].Replace("/", "")) : 1883; //1883是MQTT Server默认端口
            }
            catch (Exception)
            {
                log.Error("[MQTT]获取mqtt server端口号失败: " + connectInfo.Url);
                throw new Exception("[MQTT]获取mqtt server端口号失败");
            }
            MqttClientService mqttService = new MqttClientService(ip, port, connectInfo.UserName, connectInfo.Password);
            mqttService.OnMqttConnectNotify += MqttService_OnMqttConnectNotify;
            if (connectInfo.Type == "1")//直连
            {
                mqttService.OnMqttMessageNotify += MqttService_OnMqttMessageNotify;
            }
            else if (connectInfo.Type == "3")
            {
                // 研华网关的接入方式
                mqttService.OnMqttMessageNotify += MqttService_OnYanhuaMessageNotify;
            }
            mqttService.Connect();
            container.Add(id, mqttService);
            return mqttService;
        }

        private void MqttService_OnMqttConnectNotify(object sender, MqttConnectNotifyEventArgs e)
        {
            if (e.IsConnect)
            {
                log.Info("[MQTT] server已连接");
            }
            else {
                log.Warn("[MQTT] server连接断开");
            }

        }

        /// <summary>
        /// 通用的监听方法
        /// 工作内容：
        /// 1.根据Topic获取设备及属性实例。Topic规则：DeviceLabel/PropertyLabel
        /// 2.查询是否命中报警策略，如果有则记录报警信息
        /// 3.将实时数据保存到默认的TSDB数据库中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttService_OnMqttMessageNotify(object sender, MqttMessageNotifyEventArgs e)
        {
            log.InfoFormat("[MQTT]收到消息：客户端：{0},主题：{1},消息：{2},Qos：{3},保留：{4}",
                e.ClientId, e.MqttApplicationMessage.Topic, Encoding.UTF8.GetString(e.MqttApplicationMessage.Payload), e.MqttApplicationMessage.QualityOfServiceLevel, e.MqttApplicationMessage.Retain);
            string[] topicArry = e.MqttApplicationMessage.Topic.Split('/');
            if (topicArry.Length == 2)
            {
                string deviceLabel = topicArry[0];
                string propertyLabel = topicArry[1];
                DateTime now = DateTime.Now;

                DeviceMonitoringApi deviceMonitoringApi = new DeviceMonitoringApi();
                GetDeviceInfoParameter par = new GetDeviceInfoParameter();
                RetDeviceInfo deviceInfo = null;
                RetDeviceItemInfo itemInfo = null;
                par.DeviceLabel = deviceLabel;
                var resDeviceInfo = deviceMonitoringApi.GetDeviceInfo(par);
                if (resDeviceInfo.Code != -1)
                {
                    deviceInfo = resDeviceInfo.Data;
                    itemInfo = deviceInfo.DeviceItems.Find(s => s.PropertyLabel == propertyLabel);
                    if (null == itemInfo)
                    {
                        log.ErrorFormat("根据Topic获取设备属性信息出错，无法匹配报警策略，deviceLabel{0},propertyLabel{1}", deviceLabel, propertyLabel);
                        return;
                    }
                }
                else
                {
                    log.Error("根据deviceLabel获取设备信息失败" + resDeviceInfo.Msg);
                    return;
                }
                //异步处理报警逻辑
                AsyncHandleAlarmPolicies(deviceInfo, itemInfo, Encoding.UTF8.GetString(e.MqttApplicationMessage.Payload), now);
                //保存到TSDB
                RecordToTsdb(deviceInfo, itemInfo, Encoding.UTF8.GetString(e.MqttApplicationMessage.Payload), now);
            }
            else
            {
                log.ErrorFormat("解析TOPIC：{0} 失败，不符合规则", e.MqttApplicationMessage.Topic);
            }
        }

        /// <summary>
        /// 处理报警逻辑
        /// 1.是否命中报警策略
        /// 2.记录流水
        /// 3.发送消息
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="itemInfo"></param>
        /// <param name="curValue"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private Task<int> HandleAlarmPolicies(RetDeviceInfo deviceInfo, RetDeviceItemInfo itemInfo, string curValue, DateTime now)
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

        private async void AsyncHandleAlarmPolicies(RetDeviceInfo deviceInfo, RetDeviceItemInfo itemInfo, string curValue, DateTime now)
        {
            var result = await HandleAlarmPolicies(deviceInfo, itemInfo, curValue, now);
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

        /// <summary>
        /// 将实时数据记录到TSDB
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="itemInfo"></param>
        private void RecordToTsdb(RetDeviceInfo deviceInfo, RetDeviceItemInfo itemInfo, string curValue, DateTime now)
        {
            decimal value;
            try
            {
                value = Convert.ToDecimal(curValue);
                // 如果不能转成数字，则不存储到TSDB中。
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                return;
            }
            DeviceMonitoringApi deviceApi = new DeviceMonitoringApi();
            IoTHubConfigurationModel parameter = new IoTHubConfigurationModel();
            parameter.ID = long.Parse(deviceInfo.DataConnectID);
            var retDataConnect = deviceApi.GetDataConnect(parameter);
            if (retDataConnect.Code == -1)
            {
                log.Error("获取数据连接数据出错，ID:" + deviceInfo.DataConnectID);
                return;
            }
            RetDataConnectConfiguration dataConnect = retDataConnect.Data;
            Dictionary<string, string> tagMap = new Dictionary<string, string>();
            foreach (var tag in deviceInfo.TagList)
            {
                tagMap.Add(tag.Key, tag.Value);
            }
            try
            {
                OpentsdbClient client = new OpentsdbClient("http://" + dataConnect.ServerAddress + ":" + dataConnect.ServerPort);
                client.putData(itemInfo.PropertyLabel, now, value, tagMap);

            }
            catch (Exception)
            {
                return;
            }
        }



        /// <summary>
        /// 物接入（研华网关方式）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttService_OnYanhuaMessageNotify(object sender, MqttMessageNotifyEventArgs e)
        {
            try
            {
                log.InfoFormat("[MQTT]收到消息：客户端：{0},主题：{1},消息：{2},Qos：{3},保留：{4}",
                e.ClientId, e.MqttApplicationMessage.Topic, Encoding.UTF8.GetString(e.MqttApplicationMessage.Payload), e.MqttApplicationMessage.QualityOfServiceLevel, e.MqttApplicationMessage.Retain);
                string[] topicArry = e.MqttApplicationMessage.Topic.Split('/');
                string orgId = topicArry[0]; //组织ID

                //报文转换为对象
                YanHuaPayload yanhuaPayload;
                try
                {
                    yanhuaPayload = JsonHelper.JsonToEntity<YanHuaPayload>(Encoding.UTF8.GetString(e.MqttApplicationMessage.Payload));
                }
                catch (Exception ex)
                {
                    // 转换对象失败，报文格式不对，直接放弃
                    log.ErrorFormat("[MQTT]解析报文格式失败：{0},{1}", ex.Message, ex.StackTrace);
                    return;
                }
                foreach (var payVal in yanhuaPayload.values)
                {
                    try
                    {
                        DateTime uploadtime = DateTime.ParseExact(payVal.time, "yyyyMMdd-HH:mm:ss", CultureInfo.InvariantCulture);
                        DeviceMonitoringApi deviceMonitoringApi = new DeviceMonitoringApi();
                        GetDeviceInfoParameter par = new GetDeviceInfoParameter();
                        RetDeviceInfo deviceInfo = null;
                        RetDeviceItemInfo itemInfo = null;
                        par.OrgID = orgId;
                        par.Phone = payVal.device_ip;
                        var resDeviceInfo = deviceMonitoringApi.GetDeviceInfo(par);
                        if (resDeviceInfo.Code != -1)
                        {
                            deviceInfo = resDeviceInfo.Data;
                            itemInfo = deviceInfo.DeviceItems.Find(s => s.Name == payVal.operationValue);
                            if (null == itemInfo)
                            {
                                log.ErrorFormat("根据Topic获取设备属性信息出错，device_ip{0},name{1}", payVal.device_ip, payVal.name);
                                continue;
                            }
                        }
                        else
                        {
                            log.Error("根据deviceLabel获取设备信息失败" + resDeviceInfo.Msg);
                            continue;
                        }
                        //处理报警逻辑
                        AsyncHandleAlarmPolicies(deviceInfo, itemInfo, payVal.data, uploadtime);
                        //保存到TSDB
                        RecordToTsdb(deviceInfo, itemInfo, payVal.data, uploadtime);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                        continue;
                    }

                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("内部错误：{0},{1}", ex.Message, ex.StackTrace);
                return;
            }
            
        }
        
    }
}
