using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Emzi0767.Utilities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PinBot.Core.Notifications;

namespace PinBot.Application
{
    public class PinBotBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private DiscordClient discordClient;
        private IMediator mediator;

        public PinBotBackgroundService(IServiceScopeFactory serviceScopeFactory,
            ILogger<PinBotBackgroundService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering PinBotBackgroundService.ExecuteAsync");
            using var scope = serviceScopeFactory.CreateScope();
            discordClient = scope.ServiceProvider.GetRequiredService<DiscordClient>();

            var slashExtension = discordClient.UseSlashCommands(
                new SlashCommandsConfiguration {Services = scope.ServiceProvider});
            slashExtension.RegisterCommands<SlashCommands>();

            mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            cancellationToken.Register(OnStopping);

            discordClient.MessageReactionAdded += ReactionAddedAsync;
            discordClient.MessageReactionRemoved += ReactionRemovedAsync;
            discordClient.MessageReactionsCleared += ReactionsClearedAsync;
            discordClient.MessageDeleted += MessageDeletedAsync;
            discordClient.ChannelDeleted += ChannelDeletedAsync;
            //  discordClient.ChannelPinsUpdated += ChannelPinsUpdatedAsync; // TODO: figure out what event we need for pins
            
            await discordClient.ConnectAsync();

            await Task.Delay(-1, cancellationToken);

            void OnStopping()
            {
                discordClient.MessageReactionAdded -= ReactionAddedAsync;
                discordClient.MessageReactionRemoved -= ReactionRemovedAsync;
                discordClient.MessageReactionsCleared -= ReactionsClearedAsync;
                discordClient.MessageDeleted -= MessageDeletedAsync;
                discordClient.ChannelDeleted -= ChannelDeletedAsync;
                // discordClient.ChannelPinsUpdated -= ChannelPinsUpdatedAsync; // TODO: figure out what event we need for pins
            }
        }

        private Task ChannelDeletedAsync(DiscordClient sender, ChannelDeleteEventArgs e)
        {
            logger.LogInformation("Entering PinBotBackgroundService.ChannelDeletedAsync");
            return mediator.Publish(
                new ChannelDeletedNotification {ChannelId = e.Channel.Id}
            );
        }

        private Task MessageDeletedAsync(DiscordClient sender, MessageDeleteEventArgs e)
        {
            logger.LogInformation("Entering PinBotBackgroundService.MessageDeletedAsync");
            return mediator.Publish(
                new MessageDeletedNotification {MessageId = e.Message.Id}
            );
        }

        private Task ChannelPinsUpdatedAsync(DiscordClient sender, ChannelPinsUpdateEventArgs e)
        {
            logger.LogInformation($"Entering PinBotBackgroundService.ChannelPinsUpdatedAsync");
            if (e.Channel.GuildId == null) return Task.CompletedTask;
            return mediator.Publish(
                new ChannelPinsUpdatedNotification
                {
                    DiscordChannel = e.Channel,
                    LastPinTimestamp = e.LastPinTimestamp
                }
            );
        }

        private async Task ReactionAddedAsync(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            logger.LogInformation("Entering PinBotBackgroundService.ReactionAddedAsync");
            if (e.Channel.GuildId == null) return;
            var nonCachedMessage = await e.Channel.GetMessageAsync(e.Message.Id);
            await mediator.Publish(
                new ReactionAddedNotification(e.Emoji, nonCachedMessage, e.User)
            );
        }

        private async Task ReactionRemovedAsync(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            logger.LogInformation("Entering PinBotBackgroundService.ReactionRemovedAsync");
            if (e.Channel.GuildId == null) return;
            var nonCachedMessage = await e.Channel.GetMessageAsync(e.Message.Id);
            await mediator.Publish(
                new ReactionRemovedNotification(e.Emoji, nonCachedMessage, e.User)
            );
        }

        private Task ReactionsClearedAsync(DiscordClient sender, MessageReactionsClearEventArgs e)
        {
            logger.LogInformation("Entering PinBotBackgroundService.ReactionsClearedAsync");
            if (e.Channel.GuildId == null) return Task.CompletedTask;

            return mediator.Publish(
                new ReactionsClearedNotification(e.Message)
            );
        }
    }
}