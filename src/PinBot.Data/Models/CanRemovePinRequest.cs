namespace PinBot.Data.Models
{
    public class CanRemovePinRequest
    {
        public AuthorizedUserRequest AuthorizedUserRequest { get; set; }
        public ulong MessageId { get; set; }
    }
}