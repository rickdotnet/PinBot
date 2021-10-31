using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using MediatR;
using PinBot.Core.Notifications;

namespace PinBot.Core
{
    public class ReactionMonitor :
        INotificationHandler<ReactionAddedNotification>,
        INotificationHandler<ReactionRemovedNotification>,
        INotificationHandler<ReactionsClearedNotification>
    {
        private readonly DiscordClient discordClient;

        public ReactionMonitor(DiscordClient discordClient)
        {
            this.discordClient = discordClient;
        }

        // messageId, userId
        private static Dictionary<ulong, ulong> tempAuth = new();

        public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Message.Pinned) return;
            if (notification.Emoji.Name is not "📌") return;

            if (tempAuth.ContainsKey(notification.Message.Id))
                tempAuth[notification.Message.Id] = notification.User.Id; // should never hit
            else
                tempAuth.Add(notification.Message.Id, notification.User.Id);

            await notification.Message.PinAsync();
            await LogToPushPinChannel(
                $"{notification.User.Mention} just pinned a message in {notification.Message.Channel.Mention}");
        }

        public async Task Handle(ReactionRemovedNotification notification, CancellationToken cancellationToken)
        {
            if (!notification.Message.Pinned) return;
            if (notification.Emoji.Name is not "📌") return;

            if (tempAuth.ContainsKey(notification.Message.Id) &&
                tempAuth[notification.Message.Id] == notification.User.Id)
            {
                await notification.Message.UnpinAsync();
                tempAuth.Remove(notification.Message.Id);
                
                await LogToPushPinChannel(
                    $"{notification.User.Mention} just un-pinned a message in {notification.Message.Channel.Mention}");
            }
            else
                Console.WriteLine("Uh oh, bubba");
        }

        public Task Handle(ReactionsClearedNotification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task LogToPushPinChannel(string message)
            =>
                discordClient.Guilds.First().Value
                    .Channels.First(x => x.Key == 904452769182777405).Value
                    .SendMessageAsync(message);
    }
}