using DSharpPlus.Entities;
using MediatR;

namespace PinBot.Core.Notifications
{
    public class ReactionsClearedNotification : INotification
    {
        public DiscordMessage Message { get; }
        public DiscordUser User { get; }

        public ReactionsClearedNotification(DiscordEmoji emoji, DiscordMessage message, DiscordUser user)
        {
            Message = message;
            User = user;
        }
    }
}