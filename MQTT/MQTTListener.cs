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

        public MQTTListener(IHubContext<ChatHub> hubContext)
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883);

            client.ConnectAsync(options.Build());

            client.UseApplicationMessageReceivedHandler(async e =>
            {   
                Console.WriteLine($"Recieved on topic: {e.ApplicationMessage.Topic}");
                //if (e.ApplicationMessage.Topic == "/response")
                {
                    await hubContext.Clients.All.SendAsync("ReceiveMessage", "Arduino", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                }
            });

            client.UseConnectedHandler(async e => 
            {
                Console.WriteLine("Listener is connected to server");

                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("Test").Build());
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/response").Build());
                Console.WriteLine("Listener has subscribed to topics");
            });
        }
    }
}