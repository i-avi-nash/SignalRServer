using System;
using System.Collections.Generic;

namespace SignalRServer.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ChatUsers> ChatUsers { get; set; }
    }
}