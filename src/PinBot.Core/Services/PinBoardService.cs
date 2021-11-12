using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinBot.Core.Notifications;
using PinBot.Data;
using PinBot.Data.Models;

namespace PinBot.Core.Services
{
    public class PinBoardService : INotificationHandler<ChannelDeletedNotification>
    {
        private readonly DiscordClient discordClient;
        private readonly PinBotContext pinBotContext;

        public PinBoardService(DiscordClient discordClient, PinBotContext pinBotContext)
        {
            this.discordClient = discordClient;
            this.pinBotContext = pinBotContext;
        }

        public async Task<bool> AttachChannelToPinBoardAsync(ChannelPinBoardRequest request)
        {
            var existing = 
                await pinBotContext.
                    PinBoardMappings.
                    FirstOrDefaultAsync(
                        x=>
                            x.PinBoardChannelId == request.PinBoardChannelId &&
                            x.PinnedMessageChannelId == request.PinnedMessageChannelId &&
                            x.IsGlobalBoard == request.IsGlobalBoard
                            );
            
            if (existing != null) return true; // TODO: do we want to return true here?

            pinBotContext.PinBoardMappings.Add(request.ToEntity());
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }
        public async Task<bool> RemoveChannelFromPinBoardAsync(ChannelPinBoardRequest request)
        {
            var existing = 
                await pinBotContext.
                    PinBoardMappings.
                    FirstOrDefaultAsync(
                        x=>
                            x.PinBoardChannelId == request.PinBoardChannelId &&
                            x.PinnedMessageChannelId == request.PinnedMessageChannelId &&
                            x.IsGlobalBoard == request.IsGlobalBoard
                    );
            
            if (existing == null) return true; // TODO: do we want to return true here?

            pinBotContext.PinBoardMappings.Remove(existing);
            
            var rows = await pinBotContext.SaveChangesAsync();
            return rows > 0;
        }
        
        public async Task Handle(ChannelDeletedNotification notification, CancellationToken cancellationToken)
        {
            var existingChannels = 
                await pinBotContext.
                    PinBoardMappings.
                    Where(
                        x=>
                            x.PinBoardChannelId == notification.ChannelId ||
                            x.PinnedMessageChannelId == notification.ChannelId
                    )
                    .ToListAsync(cancellationToken);
            
            if (!existingChannels.Any()) return; // TODO: do we want to return true here?

            foreach (var channel in existingChannels)
                pinBotContext.PinBoardMappings.Remove(channel);    
            
            await pinBotContext.SaveChangesAsync(cancellationToken);
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
                try
                {
                    var channel = await discordClient.GetChannelAsync(pinBoardChannelId);
                    if (channel != null) await channel.SendMessageAsync(message);
                }
                catch (Exception ex)            
                {
                    if (ex is not NotFoundException && ex is not BadRequestException) throw;
                }
            }
        }


       
    }
}