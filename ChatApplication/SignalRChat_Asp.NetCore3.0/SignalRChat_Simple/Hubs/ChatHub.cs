using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat_Simple.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
