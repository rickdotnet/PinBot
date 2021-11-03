using System;
using System.Collections.Generic;

#nullable disable

namespace PinBot.Data.Entities
{
    public partial class Authorization
    {
        public ulong AuthorizationId { get; set; }
        public ulong UserOrRoleId { get; set; }
        public ulong GuildOrChannelId { get; set; }
    }
}
