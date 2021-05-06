using System;

using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;

namespace PersonalSite.MQTT
{
    public class MQTTBroker
    {
        IMqttServer mqttServer;
        MqttServerOptionsBuilder optionBuilder;

        public MQTTBroker(int port)
        {
            Console.WriteLine("Starting Server");

            optionBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint().WithDefaultEndpointPort(port);
            mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionBuilder.Build());
        }

        public async void stop()
        {
            await mqttServer.StopAsync();
        }
    }
}