using System;
using System.Collections.Generic;

#nullable disable

namespace PinBot.Data.Entities
{
    public partial class PinBoardMapping
    {
        public ushort PinBoardMappingId { get; set; }
        public ulong PinBoardChannelId { get; set; }
        public ulong? PinnedMessageChannelId { get; set; }
        public ulong IsGlobalBoard { get; set; }
    }
}
