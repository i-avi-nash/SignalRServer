using System;

namespace SignalRServer.Models
{
    public class ChatUsers
    {
        public Guid ChatId { get; set; }
        public string UserId { get; set; }
        //public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
    }
}
