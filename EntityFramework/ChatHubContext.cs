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
        public DbSet<ChatUsers> ChatUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatUsers>()
                .HasKey(x => new { x.ChatId, x.UserId });

            //modelBuilder.Entity<User>().HasMany(u => u.ChatUsers).WithOne(cu => cu.User)
            //    .HasForeignKey(cu => cu.UserId).HasPrincipalKey(u => u.Id).OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Chat>().HasMany(u => u.ChatUsers).WithOne(cu => cu.Chat)
            //    .HasForeignKey(cu => cu.ChatId).HasPrincipalKey(u => u.Id).OnDelete(DeleteBehavior.NoAction);
        }
    }
}