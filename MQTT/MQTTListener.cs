using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using PersonalSite.Hubs;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace PersonalSite.MQTT
{
    public class MQTTListener
    {
        private IMqttClient client;

        private static double MostRecentTemp = 0;
        private static double MostRecentHumid = 0;
        private static double MostRecentMoist = 0;
        private IHubContext<ChatHub> hubContext;

        public MQTTListener(IHubContext<ChatHub> hubContext)
        {
            this.hubContext = hubContext;

            client = new MqttFactory().CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883);
            client.ConnectAsync(options.Build());

            client.UseApplicationMessageReceivedHandler(async e =>
            {
                await ParseTopicAsync(e.ApplicationMessage.Topic, e.ApplicationMessage.Payload);
            });

            client.UseConnectedHandler(async e => 
            {
                Console.WriteLine("Listener is connected to server");
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response/temp").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response/humid").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response/moist").Build());
                Console.WriteLine("Listener has subscribed to topics");
            });
        }

        // TODO I feel like this isn't the best way to do this
        public static double GetMostRecentTemp()
        {
            return MostRecentTemp;
        }

        // TODO I feel like this isn't the best way to do this
        public static double GetMostRecentHumid()
        {
            return MostRecentHumid;
        }

        public static double GetMostRecentMoist()
        {
            return MostRecentMoist;
        }

        private async System.Threading.Tasks.Task ParseTopicAsync(string topic, byte[] payload)
        {
            string method;
            string value;
            switch(topic)
            {
                case "/response/temp":
                    MostRecentTemp = PayloadToDouble(payload);
                    value = MostRecentTemp.ToString();
                    method = "ReceiveTemp";
                    break;
                case "/response/humid":
                    MostRecentHumid = PayloadToDouble(payload);
                    value = MostRecentHumid.ToString();
                    method = "ReceiveHumid";
                    break;
                case "/response/moist":
                    MostRecentMoist = PayloadToDouble(payload);
                    value = MostRecentMoist.ToString();
                    method = "ReceiveMoist";
                    break;
                default:
                    Console.WriteLine($"Error with topic: {topic}");
                    return;
            }

            await hubContext.Clients.All.SendAsync(method, value);
        }

        private double PayloadToDouble(byte[] payload)
        {
            return Math.Round(Double.Parse(Encoding.UTF8.GetString(payload)), 2);
        }
    }
}