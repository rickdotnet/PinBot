using MediatR;

namespace PinBot.Core.Notifications
{
    public class MessageDeletedNotification : INotification
    {
        public ulong MessageId { get; set; }
    }
}