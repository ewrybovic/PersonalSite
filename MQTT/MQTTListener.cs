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

        public MQTTListener(IHubContext<ChatHub> hubContext)
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883);

            client.ConnectAsync(options.Build());

            client.UseApplicationMessageReceivedHandler(async e =>
            {   
                Console.WriteLine($"Recieved on topic: {e.ApplicationMessage.Topic}");
                if (e.ApplicationMessage.Topic.Contains("temp"))
                {
                    MostRecentTemp = Math.Round(Double.Parse(Encoding.UTF8.GetString(e.ApplicationMessage.Payload)), 2);
                    await hubContext.Clients.All.SendAsync("ReceiveTemp", MostRecentTemp.ToString());
                }
                else if (e.ApplicationMessage.Topic.Contains("humid"))
                {
                    MostRecentHumid = Math.Round(Double.Parse(Encoding.UTF8.GetString(e.ApplicationMessage.Payload)), 2);
                    await hubContext.Clients.All.SendAsync("ReceiveHumid", MostRecentHumid.ToString());
                }
                else
                    await hubContext.Clients.All.SendAsync("ReceiveMessage", "Arduino", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            });

            client.UseConnectedHandler(async e => 
            {
                Console.WriteLine("Listener is connected to server");
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response/temp").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response/humid").Build());
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
    }
}