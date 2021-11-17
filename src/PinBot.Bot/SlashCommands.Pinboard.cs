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
        [SlashCommand("add-to-pinboard", "Add this channel to a pinboard.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task AddToPinBoardCommand(InteractionContext ctx,
            [Option("Channel", "Pinboard Channel")] DiscordChannel channel)
        {
            var success = await pinBoardService.AttachChannelToPinBoardAsync(new ChannelPinBoardRequest
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
            var success = await pinBoardService.AttachChannelToPinBoardAsync(new ChannelPinBoardRequest
            {
                PinBoardChannelId = channel.Id,
                PinnedMessageChannelId = ctx.Guild.Id,
                IsGlobalBoard = true
            });

            if (success)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Set-global-pinboard success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Set-global-pinboard failure!"));
            }
        }
        
        [SlashCommand("remove-from-pinboard", "Remove this channel from a pinboard.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task RemoveFromPinBoardCommand(InteractionContext ctx,
            [Option("Channel", "Pinboard channel to remove from")] DiscordChannel channel)
        {
            var success = await pinBoardService.RemoveChannelFromPinBoardAsync(new ChannelPinBoardRequest
            {
                PinBoardChannelId = channel.Id,
                PinnedMessageChannelId = ctx.Channel.Id
            });

            if (success)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Remove-from-pinboard success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Remove-from-pinboard failure!"));
            }
        }

        [SlashCommand("remove-global-pinboard", "Set a channel as a global pinboard.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task RemoveGlobalPinBoardCommand(InteractionContext ctx,
            [Option("Channel", "Channel to remove")] DiscordChannel channel)
        {
            var success = await pinBoardService.RemoveChannelFromPinBoardAsync(new ChannelPinBoardRequest
            {
                PinBoardChannelId = channel.Id,
                IsGlobalBoard = true
            });

            if (success)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Remove-global-pinboard success!"));
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Remove-global-pinboard failure!"));
            }
        }
    }
}