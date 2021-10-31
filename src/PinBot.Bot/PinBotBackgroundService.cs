using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
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
        private IServiceScope scope;

        public PinBotBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                this.scope = scope;

                discordClient = scope.ServiceProvider.GetRequiredService<DiscordClient>();
                mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                cancellationToken.Register(OnStopping);

                discordClient.MessageReactionAdded += ReactionAdded;
                discordClient.MessageReactionRemoved += ReactionRemovedAsync;
                discordClient.MessageReactionsCleared += ReactionsClearedAsync;

                await discordClient.ConnectAsync();

                await Task.Delay(-1, cancellationToken);

                void OnStopping()
                {
                    discordClient.MessageReactionAdded -= ReactionAdded;
                    discordClient.MessageReactionRemoved -= ReactionRemovedAsync;
                    discordClient.MessageReactionsCleared -= ReactionsClearedAsync;
                }
            }
        }

        private Task ReactionsClearedAsync(DiscordClient sender, MessageReactionsClearEventArgs e)
        {
            Console.WriteLine("Reactions Cleared");
            return Task.CompletedTask;
        }

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