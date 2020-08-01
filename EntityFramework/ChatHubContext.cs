using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalRServer.Models;

namespace SignalRServer.EntityFramework
{
    public class ChatHubContext : IdentityDbContext<User>
    {
        public ChatHubContext(DbContextOptions<ChatHubContext> options) : base(options) { }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}