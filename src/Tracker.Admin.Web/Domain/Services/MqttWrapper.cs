using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using M2Mqtt;
using M2Mqtt.Messages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tracker.Admin.Web.Data;
using Tracker.Admin.Web.Dtos;

namespace Tracker.Admin.Web.Domain.Services
{
    public static class MqttWrapper
    {
        private static MqttClient _client;
        private static IMovementService _movementService;

        public static void Connect(string connectionString)
        {
            try
            {

                _client = new MqttClient("m21.cloudmqtt.com", 15480, false,
                    new X509Certificate(), new X509Certificate(), MqttSslProtocols.None);

                var code = _client.Connect(Guid.NewGuid().ToString(), "regcjdre", "sjOPZR3T1ehQ");

                var msgId = _client.Subscribe(new[] {"location/+/movement"},
                    new[] {MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE});

                //Wire up events
                _client.MqttMsgSubscribed += ClientOnMqttMsgSubscribed;
                _client.MqttMsgPublishReceived += ClientOnMqttMsgPublishReceived;

                //new up movement service
                var optionsBuilder = new DbContextOptionsBuilder();

                optionsBuilder.UseSqlServer(connectionString);
                var context = new TrackerDbContext(optionsBuilder.Options);
                _movementService = new MovementService(context);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error [Mqtt wrapper] Connect {0}", ex.Message);
            }
        }

        private static void ClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                Debug.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
                var data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(e.Message));

                DateTime swipeTimeUtc;
                var result = DateTime.TryParse(data.SwipeTime.ToString(), out swipeTimeUtc);

                swipeTimeUtc = result ? swipeTimeUtc : DateTime.UtcNow;

                var movementData = new MovementDto { DeviceId = data.DeviceId, CardUid = data.CardId, SwipeTime = swipeTimeUtc };

                _movementService.Save(movementData);
            }
            catch (Exception ex)
            {
                //todo log ex
            }
        }

        private static void ClientOnMqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Debug.WriteLine("Subscribed for id = " + e.MessageId);
        }

        public static void Disconnect()
        {
            _client.Disconnect();
        }
    }
}