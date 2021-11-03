using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using PinBot.Data;
using PinBot.Data.Entities;
using PinBot.Data.Models;

namespace PinBot.Core.Services
{
    public class AuthorizationService
    {
        private readonly PinBotContext pinBotContext;
        private readonly PinService pinService;

        public AuthorizationService(PinBotContext pinBotContext, PinService pinService)
        {
            this.pinBotContext = pinBotContext;
            this.pinService = pinService;
        }
        // auth people
        // check auth
        // remove auth

        public async Task<bool> AuthorizeIdAsync(ulong userOrRoleId, ulong guildOrChannelId)
        {
            var existing =
                await pinBotContext
                    .Authorizations
                    .Where(x => x.UserOrRoleId == userOrRoleId && x.GuildOrChannelId == guildOrChannelId)
                    .FirstOrDefaultAsync();

            if (existing != null)
                return true;

            pinBotContext.Authorizations.Add(new Authorization
            {
                UserOrRoleId = userOrRoleId,
                GuildOrChannelId = guildOrChannelId
            });

            var rows = await pinBotContext.SaveChangesAsync();

            return rows > 0;
        }

        // public Task<bool> IsAuthorizedAsync(ulong userOrRoleId, ulong guildOrChannelId)
        // {
        //     return pinBotContext.Authorizations.AnyAsync(x =>
        //         x.UserOrRoleId == userOrRoleId && x.GuildOrChannelId == guildOrChannelId);
        // }
        public Task<bool> IsAuthorizedAsync(AuthorizedUserRequest request)
        {
            if (request.IsAdmin) return Task.FromResult(true);
            if (request.Message == null) return Task.FromResult(false);
            
            return pinBotContext.Authorizations.AnyAsync(x =>
                (
                    (request.UserId != null && x.UserOrRoleId == request.UserId)
                    ||
                    (request.RoleIds != null && request.RoleIds.Any() && request.RoleIds.Contains(x.UserOrRoleId))
                )
                &&
                (
                    (request.Message.Channel.GuildId != null && x.GuildOrChannelId == request.Message.Channel.GuildId)
                    ||
                    (request.Message.ChannelId != null && x.GuildOrChannelId == request.Message.ChannelId)
                )
            );
        }

        public async Task<bool> CanRemovePinAsync(CanRemovePinRequest request)
        {
            if (request?.AuthorizedUserRequest == null) return false;
            if (!request.AuthorizedUserRequest.IsAdmin && !await IsAuthorizedAsync(request.AuthorizedUserRequest)) return false;

            var existingPin = await pinService.GetPinByMessageIdAsync(request.MessageId);
            
            // if we weren't tracking it for some reason (maybe it was before bot, or bot went down)
            // then the existingPin won't exist. Allow admins to remove, but nobody else (for now).
            if (existingPin == null && request.AuthorizedUserRequest.IsAdmin) return true;
            if (existingPin == null) return false;
            
            return existingPin.PinnedUserId == request.AuthorizedUserRequest.UserId;
        }
    }
}