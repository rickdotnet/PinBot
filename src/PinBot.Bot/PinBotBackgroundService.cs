using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PinBot.Core.Notifications;

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

            discordClient.MessageReactionAdded += ReactionAdded;
            discordClient.MessageReactionRemoved += ReactionRemovedAsync;
            discordClient.MessageReactionsCleared += ReactionsClearedAsync;
            // discordClient.MessageDeleted // TODO: make sure we clean up in the DB
            // discordClient.ChannelPinsUpdated // TODO: Track non-reaction pins and add them all the same

            await discordClient.ConnectAsync();

            await Task.Delay(-1, cancellationToken);

            void OnStopping()
            {
                discordClient.MessageReactionAdded -= ReactionAdded;
                discordClient.MessageReactionRemoved -= ReactionRemovedAsync;
                discordClient.MessageReactionsCleared -= ReactionsClearedAsync;
            }
        }

        private Task ReactionsClearedAsync(DiscordClient sender, MessageReactionsClearEventArgs e)
            => mediator.Publish(
                new ReactionsClearedNotification(e.Message)
            );

        private Task ReactionRemovedAsync(DiscordClient sender, MessageReactionRemoveEventArgs e)
            => mediator.Publish(
                new ReactionRemovedNotification(e.Emoji, e.Message, e.User)
            );

        private Task ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
            => mediator.Publish(
                new ReactionAddedNotification(e.Emoji, e.Message, e.User)
            );
    }
}