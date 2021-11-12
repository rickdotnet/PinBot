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
    public partial class SlashCommands : ApplicationCommandModule
    {
        #region Auth

        [SlashCommand("auth-user", "Auth a user to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthCommand(InteractionContext ctx, [Option("User", "User to authorize")] DiscordUser user)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.AuthorizeIdAsync(user.Id, ctx.Channel.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-user Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-user Failed!"));
            }
        }

        [SlashCommand("auth-role", "Auth a role to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthCommand(InteractionContext ctx, [Option("Role", "Role to authorize")] DiscordRole role)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.AuthorizeIdAsync(role.Id, ctx.Channel.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-role Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-role Failed!"));
            }
        }

        [SlashCommand("auth-user-global", "Auth a user to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx,
            [Option("User", "User to authorize")] DiscordUser user)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.AuthorizeIdAsync(user.Id, ctx.Guild.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-user Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-user Failed!"));
            }
        }

        [SlashCommand("auth-role-global", "Auth a role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx,
            [Option("Role", "Role to authorize")] DiscordRole role)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.AuthorizeIdAsync(role.Id, ctx.Guild.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-role Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Auth-role Failed!"));
            }
        }

        #endregion

        #region RemoveAuth

        [SlashCommand("remove-auth-user", "Remove auth from a user in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task RemoveAuthCommand(InteractionContext ctx,
            [Option("User", "User to remove auth from")] DiscordUser user)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.RemoveAuthorizedIdAsync(user.Id, ctx.Channel.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-user Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-user Failed!"));
            }
        }

        [SlashCommand("remove-auth-role", "Remove auth from a role in this channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task RemoveAuthCommand(InteractionContext ctx,
            [Option("Role", "Role to remove auth from")] DiscordRole role)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.RemoveAuthorizedIdAsync(role.Id, ctx.Channel.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-role Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-role Failed!"));
            }
        }

        [SlashCommand("remove-auth-user-global", "Remove global auth from a user or role")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task RemoveAuthFullCommand(InteractionContext ctx,
            [Option("User", "User to authorize")] DiscordUser user)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.RemoveAuthorizedIdAsync(user.Id, ctx.Guild.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-user Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-user Failed!"));
            }
        }

        [SlashCommand("remove-auth-role-global", "Remove global auth from a role")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task RemoveAuthFullCommand(InteractionContext ctx,
            [Option("Role", "Role to remove auth from")] DiscordRole role)
        {
            // TODO: see if we need to create a response first, then modify it because of timing
            var success = await authorizationService.RemoveAuthorizedIdAsync(role.Id, ctx.Guild.Id);
            if (success)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-role Success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"RemoveAuth-role Failed!"));
            }
        }

        #endregion
    }
}