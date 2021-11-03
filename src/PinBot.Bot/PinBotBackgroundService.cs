using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PinBot.Core.Notifications;
using PinBot.Data;
using PinBot.Data.Entities;

namespace PinBot.Application
{
    public class PinBotBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private DiscordClient discordClient;
        private IMediator mediator;

        public PinBotBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
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
            // discordClient.MessageDeleted // TODO: make sure we clean up in the DB
            // discordClient.ChannelPinsUpdated // TODO: Track non-reaction pins and add them all the same

            await discordClient.ConnectAsync();

            await Task.Delay(-1, cancellationToken);

            void OnStopping()
            {
                discordClient.MessageReactionAdded -= ReactionAddedAsync;
                discordClient.MessageReactionRemoved -= ReactionRemovedAsync;
                discordClient.MessageReactionsCleared -= ReactionsClearedAsync;
            }
        }

        private async Task ReactionAddedAsync(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (e.Channel.GuildId == null) return;
            var nonCachedMessage = await e.Channel.GetMessageAsync(e.Message.Id);
            await mediator.Publish(
                new ReactionAddedNotification(e.Emoji, nonCachedMessage, e.User)
            );
        }

        private async Task ReactionRemovedAsync(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            if (e.Channel.GuildId == null) return;
            var nonCachedMessage = await e.Channel.GetMessageAsync(e.Message.Id);
            await mediator.Publish(
                new ReactionRemovedNotification(e.Emoji, nonCachedMessage, e.User)
            );
        }

        private Task ReactionsClearedAsync(DiscordClient sender, MessageReactionsClearEventArgs e)
        {
            if (e.Channel.GuildId == null) return Task.CompletedTask;

            return mediator.Publish(
                new ReactionsClearedNotification(e.Message)
            );
        }
    }
}