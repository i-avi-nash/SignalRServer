using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRServer.EntityFramework;
using SignalRServer.Hubs;
using SignalRServer.Models;

namespace SignalRServer.Controllers
{
    [Route("/api/chathub")]
    public class ChatHubController : Controller
    {
        private readonly IHubContext<ChatHub> _ctx;
        private readonly ChatHubContext _context;

        public ChatHubController(IHubContext<ChatHub> ctx, ChatHubContext context)
        {
            _ctx = ctx;
            _context = context;
        }

        [HttpPost("JoinRoom")]
        public async Task<Chat> JoinRoom(string connectionId, string roomName)
        {
            var Chat = _context.Chats
                .Include(x => x.Messages)
                .OrderBy(x => x.Id)
                .FirstOrDefault(x => x.Name.ToLower() == roomName.ToLower());

            await _ctx.Groups.AddToGroupAsync(connectionId, roomName);
            return Chat;
        }

        [HttpPost("LeaveRoom")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _ctx.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("SendMessage")]
        public async Task<Message> SendMessage(int chatId, string message, string userName, string roomName)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = userName,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();

            await _ctx.Clients.Group(roomName)
                .SendAsync("receiveMessage", Message);

            return Message;
        }
    }
}