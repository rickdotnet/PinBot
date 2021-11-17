using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using PinBot.Core.Notifications;
using PinBot.Core.Services;
using PinBot.Data.Models;

namespace PinBot.Core
{
    public class DiscordMonitor :
        INotificationHandler<ReactionAddedNotification>,
        INotificationHandler<ReactionRemovedNotification>,
        INotificationHandler<ReactionsClearedNotification>,
        INotificationHandler<MessageDeletedNotification>,
        INotificationHandler<ChannelPinsUpdatedNotification>
    {
        private readonly DiscordClient discordClient;
        private readonly PinBotConfig pinBotConfig;
        private readonly AuthorizationService authorizationService;
        private readonly PinService pinService;
        private readonly PinBoardService pinBoardService;
        private readonly ILogger<DiscordMonitor> logger;

        private const string PIN_EMOJI = "📌";

        public DiscordMonitor(
            DiscordClient discordClient,
            PinBotConfig pinBotConfig,
            AuthorizationService authorizationService,
            PinService pinService,
            PinBoardService pinBoardService,
            ILogger<DiscordMonitor> logger)
        {
            this.discordClient = discordClient;
            this.pinBotConfig = pinBotConfig;
            this.authorizationService = authorizationService;
            this.pinService = pinService;
            this.pinBoardService = pinBoardService;
            this.logger = logger;
        }

        public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start Handling ReactionAddedNotification");
            if (notification.Message.Pinned) return;
            if (notification.Emoji.Name is not PIN_EMOJI) return;

            if (await authorizationService.IsAuthorizedAsync(new AuthorizedUserRequest
                {
                    IsAdmin = IsAdmin(notification),
                    UserId = notification.User.Id,
                    RoleIds = (notification.User as DiscordMember)?.Roles?.Select(x => x.Id).ToArray(),
                    Message = notification.Message
                })
            )
            {
                await notification.Message.PinAsync(); // TODO: move this to PinService
                var success = await pinService.TrackPinAsync(new AddPinRequest
                {
                    MessageId = notification.Message.Id,
                    GuildId = notification.Message.Channel.Guild.Id,
                    ChannelId = notification.Message.ChannelId,
                    PinnedUserId = notification.User.Id
                });

                if (success)
                {
                    await pinBoardService.LogToPinboardsAsync(
                        notification.Message.ChannelId,
                        $"{notification.User.Mention} just pinned a message in {notification.Message.Channel.Mention}",
                        notification.Message.Channel.GuildId
                    );
                }
            }

            logger.LogInformation("End Handling ReactionAddedNotification");
        }

        public async Task Handle(ReactionRemovedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start Handling ReactionRemovedNotification");
            if (!notification.Message.Pinned) return;
            if (notification.Emoji.Name is not PIN_EMOJI) return;

            if (await authorizationService.CanRemovePinAsync(
                    new CanRemovePinRequest
                    {
                        AuthorizedUserRequest = new AuthorizedUserRequest
                        {
                            IsAdmin = IsAdmin(notification),
                            UserId = notification.User.Id,
                            RoleIds = (notification.User as DiscordMember)?.Roles?.Select(x => x.Id).ToArray(),
                            Message = notification.Message
                        },
                        MessageId = notification.Message.Id
                    }
                )
            )
            {
                await notification.Message.UnpinAsync(); // TODO: move this to PinService
                var success = await pinService.RemovePinAsync(notification.Message.Id);

                if (success)
                {
                    // TODO: don't need to log this once pin boards are 
                    await pinBoardService.LogToPinboardsAsync(
                        notification.Message.ChannelId,
                        $"{notification.User.Mention} just pinned a message in {notification.Message.Channel.Mention}",
                        notification.Message.Channel.GuildId
                    );
                }

                logger.LogInformation("End Handling ReactionRemovedNotification");
            }
        }

        public Task Handle(ReactionsClearedNotification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // TODO: Move this to another class; it shouldn't live here
        public Task Handle(MessageDeletedNotification notification, CancellationToken cancellationToken)
        {
            return pinService.SoftDeletePinAsync(notification.MessageId);
        }

        public async Task Handle(ChannelPinsUpdatedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.LastPinTimestamp.HasValue)
            {
                var fuckshit = notification.DiscordChannel.Guild;
                var audit = (await fuckshit.GetAuditLogsAsync()).FirstOrDefault();

                var fuck = (await notification.DiscordChannel.GetPinnedMessagesAsync());
                var first = fuck.First();
                var message =
                    (await notification.DiscordChannel.GetPinnedMessagesAsync()).FirstOrDefault(x =>
                        x.Timestamp.Equals(notification.LastPinTimestamp.Value));

                //logger.LogDebug("Message is null? {0}", message == null);
                // if (message != null)
                //     await pinService.TrackPinAsync(new AddPinRequest
                //     {
                //         MessageId = message.Id,
                //         ChannelId = message.ChannelId,
                //         GuildId = notification.DiscordChannel.GuildId.Value,
                //         PinnedUserId = notification.DiscordChannel.GuildId.Value // default to guild ID until we figure something out
                //     });
            }
            else
            {
                // pin was removed
            }
        }

        // TODO: clean these duplicate methods up
        private static bool IsAdmin(ReactionAddedNotification notification)
        {
            var isAdmin =
                (
                    notification.Message.Channel.PermissionsFor(notification.User as DiscordMember)
                    & Permissions.Administrator
                )
                != Permissions.None;
            return isAdmin;
        }

        private static bool IsAdmin(ReactionRemovedNotification notification)
        {
            var isAdmin =
                (notification.Message.Channel.PermissionsFor(notification.User as DiscordMember) &
                 Permissions.Administrator) !=
                Permissions.None;
            return isAdmin;
        }
    }
}