using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using PinBot.Core;
using PinBot.Core.Services;
using PinBot.Data.Models;

namespace PinBot.Application
{
    public class SlashCommands : ApplicationCommandModule
    {
        private readonly AuthorizationService authorizationService;
        private readonly PinBoardService pinBoardService;

        public SlashCommands(AuthorizationService authorizationService, PinBoardService pinBoardService)
        {
            this.authorizationService = authorizationService;
            this.pinBoardService = pinBoardService;
        }

        [SlashCommand("auth-user", "Auth a user to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthCommand(InteractionContext ctx, [Option("User", "User to authorize")] DiscordUser user)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.AuthorizeIdAsync(user.Id, ctx.Channel.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth Failed!"));
            }
        }

        [SlashCommand("auth-role", "Auth a role to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthCommand(InteractionContext ctx, [Option("Role", "Role to authorize")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Auth not implemented! {role.Mention}"));
        }

        [SlashCommand("auth-user-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx,
            [Option("User", "User to authorize")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Auth-Full Success! {user.Mention}"));
        }

        [SlashCommand("auth-role-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx,
            [Option("Role", "Role to authorize")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Auth-Full not implemented! {role.Mention}"));
        }

        [SlashCommand("add-to-pinboard", "Add this channel to a pinboard.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AddToPinBoardCommand(InteractionContext ctx,
            [Option("Channel", "Pinboard Channel")] DiscordChannel channel)
        {
            var success = await pinBoardService.AttachChannelToPinBoardAsync(new AttachChannelToPinBoardRequest
            {
                PinBoardChannelId = channel.Id,
                PinnedMessageChannelId = ctx.Channel.Id
            });

            if (success)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Add-to-pinboard success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Add-to-pinboard failure!"));
            }
        }

        [SlashCommand("set-global-pinboard", "Set a channel as a global pinboard.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task SetGlobalPinBoardCommand(InteractionContext ctx,
            [Option("Channel", "Channel to set")] DiscordChannel channel)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"set-global-pinboard not implemented!"));
        }
    }
}