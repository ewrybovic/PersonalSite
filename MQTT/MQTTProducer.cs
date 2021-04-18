using System;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace PersonalSite.MQTT
{
    public static class MQTTProducer
    {
        private static IMqttClient client;

        public static async void init()
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883);

            await client.ConnectAsync(options.Build());
            client.UseConnectedHandler(e => 
            {
                Console.WriteLine("Producer is connected to server");
            });
            
            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Producer has been disconnected from the server");    
            });
        }

        public static async Task sendMessage(string user, string message)
        {
            //Console.WriteLine("Sending message");
            var appMessage = new MqttApplicationMessageBuilder()
            .WithTopic("/request");

            switch(message)
            {
                case "on":
                    appMessage.WithPayload( new byte[] {1} );
                    break;
                case "off":
                    appMessage.WithPayload( new byte[] {0} );
                    break;
                case "state":
                    appMessage.WithPayload( new byte[] {2} );
                    break;
                default:
                    appMessage.WithPayload(message);
                    break;
            }

            await client.PublishAsync(appMessage.Build());
        }


    }
}