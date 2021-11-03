using System;
using System.Collections.Generic;


namespace PinBot.Data.Entities
{
    public partial class Pin
    {
        
        public ulong PinId { get; set; }
        public ulong MessageId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong PinnedUserId { get; set; }
        public bool IsMessageDeleted { get; set; }
        public bool IsPinRemoved { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
