using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PinBot.Core.Notifications;

namespace PinBot.Core
{
    public class ReactionMonitor :
        INotificationHandler<ReactionAddedNotification>,
        INotificationHandler<ReactionRemovedNotification>,
        INotificationHandler<ReactionsClearedNotification>
    {
        // messageId, userId
        private static Dictionary<ulong, ulong> tempAuth = new();

        // private int test = 0;
        // private static int staticTest = 0;
        public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Message.Pinned) return;
            if (notification.Emoji.Name is not "📌") return;

            if (tempAuth.ContainsKey(notification.Message.Id))
                tempAuth[notification.Message.Id] = notification.User.Id; // should never hit
            else
                tempAuth.Add(notification.Message.Id, notification.User.Id);

            await notification.Message.PinAsync();
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
            }
            else
                Console.WriteLine("Uh oh, bubba");
        }

        public Task Handle(ReactionsClearedNotification notification, CancellationToken cancellationToken)
        {
            // test += 1;
            // staticTest += 1;

            return Task.CompletedTask;
        }
    }
}