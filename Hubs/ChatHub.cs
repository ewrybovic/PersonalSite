using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

using PersonalSite.MQTT;

namespace PersonalSite.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await MQTTProducer.sendMessage(user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task RequestMostRecent()
        {
            await Clients.Caller.SendAsync("ResponseMostRecent", MQTTListener.GetMostRecentHumid(), MQTTListener.GetMostRecentTemp(), MQTTListener.GetMostRecentMoist());
        }
    }
}