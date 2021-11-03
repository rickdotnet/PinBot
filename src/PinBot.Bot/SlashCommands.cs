using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using PinBot.Core;
using PinBot.Core.Services;

namespace PinBot.Application
{
    
    public class SlashCommands : ApplicationCommandModule
    {
        private readonly AuthorizationService authorizationService;

        public SlashCommands(AuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
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
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth Success! {role.Mention}"));
        }
        [SlashCommand("auth-user-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx, [Option("User", "User to authorize")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth-Full Success! {user.Mention}"));
        }
        [SlashCommand("auth-role-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task AuthFullCommand(InteractionContext ctx, [Option("Role", "Role to authorize")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth-Full Success! {role.Mention}"));
        }
    }
}