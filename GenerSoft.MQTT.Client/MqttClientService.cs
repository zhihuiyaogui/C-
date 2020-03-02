using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GenerSoft.MQTT.Client
{
    public class MqttClientService
    {
        #region Field&Attribute
        private static MqttClientService _instance = null;
        private static readonly object lockObject = new object();

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string ClientId { get; private set; }
        public bool IsConnected { get; private set; }

        public bool ReallyConnected() {
            return this.mqttClient.IsConnected;
        }

        private MqttQualityOfServiceLevel MqttQualityOfServiceLevel { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public MqttClient mqttClient { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #endregion

        public MqttClientService(string ipAddress, int port, string userName, string password)
        {
            if (mqttClient == null)
            {
                try
                {
                    InitOptions(ProtocolType.TCP, ipAddress, port, userName, password,MqttQualityOfServiceLevel.AtMostOnce);

                    ClientId = Guid.NewGuid().ToString();
                    mqttClient = new MqttFactory().CreateMqttClient() as MqttClient;
                    mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived; ;
                    mqttClient.Connected += MqttClient_Connected; ;
                    mqttClient.Disconnected += MqttClient_Disconnected; ;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 单例的情况
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static MqttClientService CreateInstance(string ipAddress, int port, string userName, string password)
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new MqttClientService(ipAddress,port,userName,password);
                    }
                }
            }
            return _instance;
        }

        #region Functions

        public void InitOptions(ProtocolType protocolType, string ipAddress, int port, string userName, string password, MqttQualityOfServiceLevel mqttQuality = MqttQualityOfServiceLevel.AtMostOnce)
        {
            IsConnected = false;
            IpAddress = ipAddress;
            Port = port;
            ProtocolType = protocolType;
            MqttQualityOfServiceLevel = mqttQuality;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            try
            {
                mqttClient.ConnectAsync(CreateOptions());
                IsConnected = true;
            }
            catch (MqttCommunicationException ee)
            {

                throw ee;
            }

        }
        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect()
        {
            try
            {
                mqttClient.DisconnectAsync();
                IsConnected = false;
            }
            catch (MqttCommunicationException ee)
            {
                throw ee;
            }

        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public async void PublishMessage(string topic, string message)
        {
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }
            if (IsConnected)
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel)
                .WithRetainFlag(true)//保持标志（Retain-Flag）该标志确定代理是否持久保存某个特定主题的消息。订阅该主题的新客户端将在订阅后立即收到该主题的最后保留消息。
                .Build();

                await mqttClient.PublishAsync(applicationMessage);
            }
        }
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic"></param>
        public async void SubscribeMessage(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }
            int tryTimes = 0;
            while (tryTimes < 5) {
                if (mqttClient.IsConnected)
                {
                    await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).WithQualityOfServiceLevel(MqttQualityOfServiceLevel).Build());
                    log.InfoFormat("[MQTT]Topic:{0} subscribe success!", topic);
                    return;
                }
                else {
                    log.Warn("[MQTT]Server is not connected, wait 1s,try times:" + tryTimes);
                    Thread.Sleep(1000);
                    tryTimes += 1;
                }
            }
            log.Error("[MQTT]ERROR!Try to subscribe topic failed!Topic: " + topic);

        }

        /// <summary>
        /// 批量订阅主题
        /// </summary>
        /// <param name="topicList"></param>
        public void batchSubscribeMessage(List<string> topicList) {
            if (null != topicList &&topicList.Count !=0)
            {
                foreach (string item in topicList)
                {
                    SubscribeMessage(item);
                }
            }
        }

        /// <summary>
        /// 批量取消订阅主题
        /// </summary>
        /// <param name="topicList"></param>
        public void batchUnsubscribeMaessage(List<string> topicList) {
            if (null != topicList && topicList.Count != 0)
            {
                foreach (string item in topicList)
                {
                    UnsubscribeMessage(item);
                }
            }
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topic"></param>
        public async void UnsubscribeMessage(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }
            await mqttClient.UnsubscribeAsync(topic);
            log.InfoFormat("[MQTT]Topic:{0} Unsubscribe success!", topic);
        }

        private MqttClientOptions CreateOptions()
        {
            try
            {
                //启用TLS
                var tlsOptions = new MqttClientTlsOptions
                {
                    UseTls = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    AllowUntrustedCertificates = true
                };
                var options = new MqttClientOptions
                {
                    ClientId = ClientId,
                };
                switch (ProtocolType)
                {
                    case ProtocolType.TCP:
                        options.ChannelOptions = new MqttClientTcpOptions
                        {
                            Server = IpAddress,
                            Port = Port,
                            //TlsOptions = tlsOptions
                        };
                        break;
                    case ProtocolType.WS:
                        options.ChannelOptions = new MqttClientWebSocketOptions
                        {
                            Uri = IpAddress,
                            TlsOptions = tlsOptions
                        };
                        break;
                    default:
                        break;
                }
                if (options.ChannelOptions == null)
                {
                    throw new InvalidOperationException();
                }
                //设定证书
                options.Credentials = new MqttClientCredentials
                {
                    Username = UserName,
                    Password = Password
                };

                options.CleanSession = true;//会话清除
                options.KeepAlivePeriod = TimeSpan.FromSeconds(10);


                //ProtocolVersion = MQTTnet.Serializer.MqttProtocolVersion.V311,
                return options;
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Events
        public event EventHandler<MqttConnectNotifyEventArgs> OnMqttConnectNotify;
        public event EventHandler<MqttMessageNotifyEventArgs> OnMqttMessageNotify;

        private void MqttClient_Disconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            if (OnMqttConnectNotify != null)
            {
                OnMqttConnectNotify(sender, new MqttConnectNotifyEventArgs(false));
            }
        }

        private void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            if (OnMqttConnectNotify != null)
            {

                OnMqttConnectNotify(sender, new MqttConnectNotifyEventArgs(true));
            }

        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (OnMqttMessageNotify != null)
            {
                OnMqttMessageNotify(sender, new MqttMessageNotifyEventArgs(true, e.ClientId, e.ApplicationMessage));
            }
        }
        #endregion

        


    }
}
