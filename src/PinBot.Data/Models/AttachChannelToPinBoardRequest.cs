using PinBot.Data.Entities;

namespace PinBot.Data.Models
{
    public class ChannelPinBoardRequest
    {
        public ulong PinBoardChannelId { get; set; }
        public ulong? PinnedMessageChannelId { get; set; }
        public bool IsGlobalBoard { get; set; }
        public PinBoardMapping ToEntity()
            => new()
            {
                PinBoardChannelId = PinBoardChannelId,
                PinnedMessageChannelId = PinnedMessageChannelId,
                IsGlobalBoard = IsGlobalBoard
            };
    }
}