using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using PinBot.Core;

namespace PinBot.Application
{
    
    public class SlashCommands : ApplicationCommandModule
    {
        public SlashCommands(PinBotConfig config)
        {
            var test = config.TestString;
        }
        [SlashCommand("auth-user", "Auth a user to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AuthCommand(InteractionContext ctx, [Option("User", "User to authorize")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth Success! {user.Mention}"));
        }
        [SlashCommand("auth-role", "Auth a role to pin messages in this channel")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AuthCommand(InteractionContext ctx, [Option("Role", "Role to authorize")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth Success! {role.Mention}"));
        }
        [SlashCommand("auth-user-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AuthFullCommand(InteractionContext ctx, [Option("User", "User to authorize")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth-Full Success! {user.Mention}"));
        }
        [SlashCommand("auth-role-full", "Auth a user or role to pin messages in ANY channel")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AuthFullCommand(InteractionContext ctx, [Option("Role", "Role to authorize")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Auth-Full Success! {role.Mention}"));
        }
    }
}