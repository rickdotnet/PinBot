using System;
using PinBot.Data.Entities;

namespace PinBot.Data.Models
{
    public class AddPinRequest
    {
        public ulong MessageId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong PinnedUserId { get; set; }

        public Pin ToEntity()
            => new()
            {
                MessageId = MessageId,
                GuildId = GuildId,
                ChannelId = ChannelId,
                PinnedUserId = PinnedUserId,
                Timestamp = DateTime.Now
            };
    }
}