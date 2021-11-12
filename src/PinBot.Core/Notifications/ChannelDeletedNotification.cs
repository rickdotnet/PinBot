using MediatR;

namespace PinBot.Core.Notifications
{
    public class ChannelDeletedNotification : INotification
    {
        public ulong ChannelId { get; set; }
    }
}