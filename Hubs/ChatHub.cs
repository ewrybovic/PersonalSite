using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PersonalSite.Data;
using PersonalSite.Models;
using PersonalSite.MQTT;

namespace PersonalSite.Hubs
{
    public class ChatHub : Hub
    {

        PersonalSiteContext _context;

        public ChatHub(PersonalSiteContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            User userEntry = await _context.User.FirstOrDefaultAsync(m => m.Username == user);

            if (userEntry != null && userEntry.IsAuthorized)
            {
                await MQTTProducer.sendMessage(user, message);
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "You are not authorized");
            }
        }

        public async Task RequestMostRecent()
        {
            await Clients.Caller.SendAsync("ResponseMostRecent", MQTTListener.GetMostRecentHumid(), MQTTListener.GetMostRecentTemp(), MQTTListener.GetMostRecentMoist());
        }
    }
}