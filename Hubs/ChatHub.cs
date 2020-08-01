using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connection Established: " + Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnectionID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}