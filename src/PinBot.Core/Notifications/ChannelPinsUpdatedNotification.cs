using System;
using DSharpPlus.Entities;
using MediatR;

namespace PinBot.Core.Notifications
{
    public class ChannelPinsUpdatedNotification : INotification
    {
         public DiscordChannel DiscordChannel { get; set; }
         public DateTimeOffset? LastPinTimestamp { get; set; }
    }
}