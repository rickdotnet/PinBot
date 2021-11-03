using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using PinBot.Data;
using PinBot.Data.Models;

namespace PinBot.Core.Services
{
    public class PinBoardService
    {
        private readonly DiscordClient discordClient;
        private readonly PinBotContext pinBotContext;

        public PinBoardService(DiscordClient discordClient, PinBotContext pinBotContext)
        {
            this.discordClient = discordClient;
            this.pinBotContext = pinBotContext;
        }

        public async Task<bool> AttachChannelToPinBoardAsync(AttachChannelToPinBoardRequest request)
        {
            if (!request.PinnedMessageChannelId.HasValue) return false;
            var existing = 
                await pinBotContext.
                    PinBoardMappings.
                    FirstOrDefaultAsync(
                        x=>
                            x.PinBoardChannelId == request.PinBoardChannelId &&
                            x.PinnedMessageChannelId == request.PinnedMessageChannelId
                            );
            if (existing != null) return true; // TODO: do we want to return true here?

            pinBotContext.PinBoardMappings.Add(request.ToEntity());
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }

        public async Task LogToPinboardsAsync(ulong channelId, string message)
        {
            var channelIds = await pinBotContext
                .PinBoardMappings
                .Where(x =>x.IsGlobalBoard || x.PinnedMessageChannelId == channelId)
                .Select(x=>x.PinBoardChannelId)
                .ToListAsync();

            foreach (var pinBoardChannelId in channelIds)
            {
                var channel = await discordClient.GetChannelAsync(pinBoardChannelId);
                if (channel != null) await channel.SendMessageAsync(message);
            }
        }
    }
}