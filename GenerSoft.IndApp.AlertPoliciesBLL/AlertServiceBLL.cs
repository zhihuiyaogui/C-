using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.CommonSdk.Model.Device.DeviceMonitoring;
using GenerSoft.MQTT.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenerSoft.IndApp.AlertPoliciesBLL
{
    public class AlertServiceBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async static void  AlertServiceStart()
        {
            string res = await Run();
            return;
        }


        private static Task<string> Run() {
            return Task.Run(() => { starter(); return ""; });
        }

        private static void starter() {
            log.Info("[MQTT]Service Start ...");
            MqttServiceContainer container = MqttServiceContainer.Instance;
            List<RetDeviceInfo> deviceInfo = new List<RetDeviceInfo>();
            DeviceMonitoringApi deviceApi = new DeviceMonitoringApi();
            var retDevice = deviceApi.GetAllDeviceList(new GetDeviceInfoParameter() { Status = "1"});
            if (retDevice.Code == -1)
            {
                log.Error("获取设备信息失败： " + retDevice.Msg);
                return;
            }
            else
            {
                deviceInfo = retDevice.Data;
            }

            foreach (var item in deviceInfo)
            {
                StartUpDeviceService(item);
            }
            log.Info("[MQTT]Service Started Success!");
            return;
        }


        /// <summary>
        /// 启动设备的MQTT监听服务
        /// </summary>
        /// <param name="item"></param>
        public static void StartUpDeviceService(RetDeviceInfo item) {
            // 物接入
            if (item.ConnectType == "0")
            {
                //判断物接入的方式
                IoTHubConfigurationModel model = new IoTHubConfigurationModel();
                model.ID = long.Parse(item.IoTHubID);
                DeviceMonitoringApi deviceApi = new DeviceMonitoringApi();
                var conf = deviceApi.GetIoTHubConnection(model);
                if (conf.Code == -1)
                {
                    //throw new Exception("获取IoTHub连接信息失败" + conf.Msg);
                    log.Error("获取IoTHub连接信息失败" + conf.Msg + " ;model ID" + model.ID);
                    return;
                }
                RetIoTHubConfiguration connectInfo = (RetIoTHubConfiguration)conf.Data;
                long ioTHubID = long.Parse(item.IoTHubID);
                //====================
                //=====直接接入=======
                //====================
                if (connectInfo.Type == "1") 
                {
                    try
                    {
                        MqttClientService service = MqttServiceContainer.Instance.GetMqttServiceByConnectID(ioTHubID);
                        //订阅该设备下的相关属性TOPIC
                        BatchSubMessage(item, service);
                        log.InfoFormat("[MQTT] Device: {0},Service Enable.", item.Name);
                    }
                    catch (Exception e)
                    {
                        log.Error("获取MQTT Client出错:" + e.Message, e);
                    }
                }
                //==========================
                //==研华网关的设备直连方式==
                //==========================
                else if (connectInfo.Type == "3") 
                {
                    try
                    {
                        MqttClientService service = MqttServiceContainer.Instance.GetMqttServiceByConnectID(ioTHubID);
                        //订阅该设备下的相关属性TOPIC
                        //todo: 暂时使用remark字段存储订阅的topic
                        service.SubscribeMessage(item.Remark);
                        log.InfoFormat("[MQTT] Device: {0},Service Enable.", item.Name);
                    }
                    catch (Exception e)
                    {
                        log.Error("获取MQTT Client出错:" + e.Message, e);
                    }
                }
            }
        }


        /// <summary>
        /// 批量订阅设备下TOPIC
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="service"></param>
        private static void BatchSubMessage(RetDeviceInfo deviceInfo, MqttClientService service)
        {
            List<string> toSubList = new List<string>();
            if (null != deviceInfo.DeviceItems && deviceInfo.DeviceItems.Count > 0)
            {
                foreach (var deviceItem in deviceInfo.DeviceItems)
                {
                    toSubList.Add(deviceInfo.DeviceLabel + "/" + deviceItem.PropertyLabel);
                }
            }
            service.batchSubscribeMessage(toSubList);
        }

        /// <summary>
        /// 禁用设备的处理
        /// </summary>
        /// <param name="id"></param>
        public void DisableDeivce(long deviceId)
        {

            DeviceMonitoringApi deviceMonitoringApi = new DeviceMonitoringApi();
            GetDeviceInfoParameter par = new GetDeviceInfoParameter();
            par.ID = deviceId.ToString();
            RetDeviceInfo deviceInfo = null;
            var resDeviceInfo = deviceMonitoringApi.GetDeviceInfo(par);
            if (resDeviceInfo.Code != -1)
            {
                deviceInfo = resDeviceInfo.Data;
                if (MqttServiceContainer.Instance.IsClientExist(long.Parse(deviceInfo.IoTHubID)))
                {

                    RetIoTHubConfiguration connectInfo = GetConnectInfoById(deviceInfo.IoTHubID);
                    if (null!=connectInfo)
                    {
                        if (connectInfo.Type == "1")
                        {
                            //设备直连，删除Topics 
                            BatchUnsubMessage(deviceInfo, MqttServiceContainer.Instance.GetMqttServiceByConnectID(long.Parse(deviceInfo.IoTHubID)));
                        }
                        else if (connectInfo.Type == "3") {
                            //研华网关，删除remark中的topic
                            MqttServiceContainer.Instance.GetMqttServiceByConnectID(long.Parse(deviceInfo.IoTHubID)).UnsubscribeMessage(deviceInfo.Remark);
                        }
                    }

                    log.InfoFormat("[MQTT] Device {0},Service Disable.", deviceInfo.Name);
                }

            }
            else
            {
                log.Error("获取设备信息出错：" + resDeviceInfo.Msg);
                return;
            }

        }

        /// <summary>
        /// 启用设备
        /// </summary>
        /// <param name="deviceId"></param>
        public void EnableDevice(long deviceId) {
            DeviceMonitoringApi deviceMonitoringApi = new DeviceMonitoringApi();
            GetDeviceInfoParameter par = new GetDeviceInfoParameter();
            par.ID = deviceId.ToString();
            RetDeviceInfo deviceInfo = null;
            var resDeviceInfo = deviceMonitoringApi.GetDeviceInfo(par);
            if (resDeviceInfo.Code != -1)
            {
                deviceInfo = resDeviceInfo.Data;
                StartUpDeviceService(deviceInfo);
                
            }
            else
            {
                log.Error("获取设备信息出错：" + resDeviceInfo.Msg);
                return;
            }
        }

        /// <summary>
        /// 批量取消订阅
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="service"></param>
        private static void BatchUnsubMessage(RetDeviceInfo deviceInfo, MqttClientService service)
        {
            List<string> toSubList = new List<string>();
            if (null != deviceInfo.DeviceItems && deviceInfo.DeviceItems.Count > 0)
            {
                foreach (var deviceItem in deviceInfo.DeviceItems)
                {
                    toSubList.Add(deviceInfo.DeviceLabel + "/" + deviceItem.PropertyLabel);
                }
            }
            service.batchUnsubscribeMaessage(toSubList);
        }


        public RetIoTHubConfiguration GetConnectInfoById(string ioTHubID) {
            IoTHubConfigurationModel model = new IoTHubConfigurationModel();
            model.ID = long.Parse(ioTHubID);
            DeviceMonitoringApi deviceApi = new DeviceMonitoringApi();
            var conf = deviceApi.GetIoTHubConnection(model);
            if (conf.Code == -1)
            {
                //throw new Exception("获取IoTHub连接信息失败" + conf.Msg);
                log.Error("获取IoTHub连接信息失败" + conf.Msg + " ;model ID" + model.ID);
                return null;
            }
            RetIoTHubConfiguration connectInfo = (RetIoTHubConfiguration)conf.Data;
            return connectInfo;
        }
    }
}
