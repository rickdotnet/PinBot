using DSharpPlus.Entities;
using MediatR;

namespace PinBot.Core.Notifications
{
    public class ReactionsClearedNotification : INotification
    {
        public DiscordMessage Message { get; }

        public ReactionsClearedNotification(DiscordMessage message)
        {
            Message = message;
        }
    }
}