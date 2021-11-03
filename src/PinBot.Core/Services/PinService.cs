using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinBot.Data;
using PinBot.Data.Entities;
using PinBot.Data.Models;

namespace PinBot.Core.Services
{
    public class PinService
    {
        private readonly PinBotContext pinBotContext;

        public PinService(PinBotContext pinBotContext)
        {
            this.pinBotContext = pinBotContext;
        }

        public Task<Pin> GetPinByMessageIdAsync(ulong messageId)
        {
            return pinBotContext.Pins.SingleOrDefaultAsync(x =>
                x.MessageId == messageId && !x.IsPinRemoved && !x.IsMessageDeleted);
        }

        public async Task<bool> TrackPinAsync(AddPinRequest request)
        {
            var existing = await GetPinByMessageIdAsync(request.MessageId);
            if (existing != null) return true; // TODO: do we want to return true here?

            pinBotContext.Pins.Add(request.ToEntity());
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }
        public async Task<bool> RemovePinAsync(ulong messageId)
        {
            var existing = await GetPinByMessageIdAsync(messageId);
            if (existing == null) return false;

            existing.IsPinRemoved = true;
            existing.Timestamp = DateTime.Now;
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }
        public async Task<bool> SoftDeletePinAsync(ulong messageId)
        {
            var existing = await GetPinByMessageIdAsync(messageId);
            if (existing == null) return false;

            existing.IsMessageDeleted = true;
            existing.Timestamp = DateTime.Now;
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }
    }
}