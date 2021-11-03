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
    public class ReactionMonitor :
        INotificationHandler<ReactionAddedNotification>,
        INotificationHandler<ReactionRemovedNotification>,
        INotificationHandler<ReactionsClearedNotification>
    {
        private readonly DiscordClient discordClient;
        private readonly PinBotConfig pinBotConfig;
        private readonly AuthorizationService authorizationService;
        private readonly PinService pinService;
        private readonly ILogger<ReactionMonitor> logger;

        private const string PIN_EMOJI = "📌";

        public ReactionMonitor(DiscordClient discordClient, PinBotConfig pinBotConfig,
            AuthorizationService authorizationService,
            PinService pinService,
            ILogger<ReactionMonitor> logger)
        {
            this.discordClient = discordClient;
            this.pinBotConfig = pinBotConfig;
            this.authorizationService = authorizationService;
            this.pinService = pinService;
            this.logger = logger;
        }

        // messageId, userId
        //private static Dictionary<ulong, ulong> tempAuth = new();

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
                    GuildId = notification.Message.Channel.GuildId,
                    ChannelId = notification.Message.ChannelId
                })
            )
            {
                await notification.Message.PinAsync();
                var success = await pinService.TrackPinAsync(new AddPinRequest
                {
                    MessageId = notification.Message.Id,
                    GuildId = notification.Message.Channel.Guild.Id,
                    ChannelId = notification.Message.ChannelId,
                    PinnedUserId = notification.User.Id
                });

                if (success)
                {
                    await LogToPushPinChannel(
                        $"{notification.User.Mention} just pinned a message in {notification.Message.Channel.Mention}");
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
                            GuildId = notification.Message.Channel.GuildId,
                            ChannelId = notification.Message.ChannelId
                        },
                        MessageId = notification.Message.Id
                    }
                )
            )
            {
                await notification.Message.UnpinAsync();
                await pinService.RemovePinAsync(notification.Message.Id);

                await LogToPushPinChannel(
                    $"{notification.User.Mention} just un-pinned a message in {notification.Message.Channel.Mention}");
                
                logger.LogInformation("End Handling ReactionRemovedNotification");
            }
        }

        public Task Handle(ReactionsClearedNotification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task LogToPushPinChannel(string message)
            =>
                discordClient.Guilds.First().Value
                    .Channels.First(x => x.Key == pinBotConfig.TempChannelId)
                    .Value // TODO: this needs to be swapped to "PinBoard"
                    .SendMessageAsync(message);
        // TODO: clean these duplicate methods up
        private static bool IsAdmin(ReactionAddedNotification notification)
        {
            var isAdmin =
                (notification.Message.Channel.PermissionsFor(notification.User as DiscordMember) &
                 Permissions.Administrator) !=
                Permissions.None;
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