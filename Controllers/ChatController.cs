using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRServer.EntityFramework;
using SignalRServer.Hubs;
using SignalRServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Controllers
{
    [Route("/api/chat")]
    public class ChatController : Controller
    {
        private readonly ChatHubContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _ctx;

        public ChatController(ChatHubContext context, UserManager<User> userManager, IHubContext<ChatHub> ctx)
        {
            _context = context;
            _userManager = userManager;
            _ctx = ctx;
        }

        [HttpGet("LoadChats")]
        public async Task<List<Chat>> GetUserChats(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var chats = await _context.Chats.Include(c => c.ChatUsers)
                    .Where(c => c.ChatUsers.Any(cu => cu.UserId == user.Id))
                    .ToListAsync();

                return chats;
            }
            return null;
        }

        [HttpPost("CreateChat")]
        public Chat CreateChat(string userId, Guid chatId)
        {
            if (chatId == Guid.Empty)
            {
                var newChat = new Chat
                {
                    Id = Guid.NewGuid(),
                    Name = "Private",
                    ChatUsers = new List<ChatUsers>
                    {
                        new ChatUsers
                        {
                            UserId = userId
                        }
                    }
                };

                var chat = _context.Chats.Add(newChat);
                _context.SaveChanges();

                return chat.Entity;
            }
            var chatUpdate = _context.Chats.Include(c => c.ChatUsers).FirstOrDefault(c => c.Id == chatId);

            chatUpdate.ChatUsers.Add(new ChatUsers { UserId = userId, ChatId = chatId });
            _context.SaveChanges();

            return chatUpdate;
        }

        [HttpPost("JoinChat")]
        public async Task<Chat> JoinChat(string sender, string receiver, string connectionId)
        {
            //if both already opened a private chat before
            var chats = _context.Chats.Include(c => c.ChatUsers).Include(c => c.Messages)
                .Where(c => c.ChatUsers.Any(cu => cu.UserId == sender) && c.Name.ToLower() == "private")
                .ToList();

            var result = chats.Count != 0 ? chats.FirstOrDefault(c => c.ChatUsers.Any(ch => ch.UserId == receiver)) : null;

            if (result != null)
            {
                // ------------->Don't call backend untill user sends a message first<--------------
                //handles first message send here
                await _ctx.Groups.AddToGroupAsync(connectionId, result.Id.ToString());

                //add both user to chat group with chatid as group-name

                //go and open chat windows
                //load previoud chats between sender and reciever
                //send notification to reciever about new message [if message is sent from sender]

                var messages = _context.Messages.Where(m => m.ChatId == result.Id).OrderBy(m => m.Timestamp).ToList();

                //return chats.FirstOrDefault(c => c.Id == result.Id);
                return new Chat { Id = result.Id, Name = result.Name, Messages = messages };
            }

            var senderChat = CreateChat(sender, Guid.Empty);
            var receiverChat = CreateChat(receiver, senderChat.Id);
            //open new chat windows [create userchat record for both]
            //inform receiver about new conversation with notification
            return _context.Chats.Include(c => c.ChatUsers).FirstOrDefault(c => c.Id == senderChat.Id);
        }

    }
}
