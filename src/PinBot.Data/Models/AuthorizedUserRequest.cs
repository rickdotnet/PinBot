namespace PinBot.Data.Models
{
    public class AuthorizedUserRequest
    {
        public ulong UserId { get; set; }
        public ulong[] RoleIds { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? ChannelId { get; set; }
        public bool IsAdmin { get; set; }
    }
}