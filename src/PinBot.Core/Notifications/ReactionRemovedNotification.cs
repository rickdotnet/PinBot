using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;

namespace PinBot.Core.Notifications
{
    public class ReactionRemovedNotification : INotification
    {
        public DiscordEmoji Emoji { get; }
        public DiscordMessage Message { get; }
        
        public DiscordUser User { get; }
        
        public ReactionRemovedNotification(DiscordEmoji emoji, DiscordMessage message, DiscordUser user)
        {
            Emoji = emoji;
            Message = message;
            User = user;
        }
    }
}