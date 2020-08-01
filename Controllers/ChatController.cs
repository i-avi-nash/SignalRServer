using Microsoft.AspNetCore.Mvc;
using SignalRServer.EntityFramework;
using SignalRServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Controllers
{
    [Route("/api/chat")]
    public class ChatController : Controller
    {
        private readonly ChatHubContext _context;

        public ChatController(ChatHubContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllRoom")]
        public List<Chat> GetAllRoom()
        {
            var Chats = _context.Chats.ToList();

            return Chats;
        }

        [HttpGet("GetRoom")]
        public Chat GetRoom(string roomName)
        {
            var Chat = _context.Chats
                .FirstOrDefault(x => x.Name.ToLower() == roomName.ToLower());

            return Chat;
        }

        [HttpPost("CreateRoom")]
        public async Task<Chat> CreateRoom(string roomName)
        {
            var Chat = new Chat
            {
                Name = roomName
            };
            await _context.Chats.AddAsync(Chat);
            await _context.SaveChangesAsync();

            return Chat;
        }
    }
}
