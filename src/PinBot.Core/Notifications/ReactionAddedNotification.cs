using DSharpPlus.Entities;
using MediatR;

namespace PinBot.Core.Notifications
{
    public class ReactionAddedNotification : INotification
    {
        public DiscordEmoji Emoji { get; }
        public DiscordMessage Message { get; }
        public DiscordUser User { get; }

        public ReactionAddedNotification(DiscordEmoji emoji, DiscordMessage message, DiscordUser user)
        {
            Emoji = emoji;
            Message = message;
            User = user;
        }
    }
}