using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinBot.Data;
using PinBot.Data.Models;

namespace PinBot.Core.Services
{
    public class PinBoardService
    {
        private readonly PinBotContext pinBotContext;

        public PinBoardService(PinBotContext pinBotContext)
        {
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
    }
}