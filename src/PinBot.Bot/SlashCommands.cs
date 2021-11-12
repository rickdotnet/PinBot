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
        private readonly AuthorizationService authorizationService;
        private readonly PinBoardService pinBoardService;

        public SlashCommands(AuthorizationService authorizationService, PinBoardService pinBoardService)
        {
            this.authorizationService = authorizationService;
            this.pinBoardService = pinBoardService;
        }
        
        // auth and pinboard commands are in partials
    }
}