using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iyoku.Modules
{
    class CommunicationModule : ModuleBase
    {
        [Command("Who are you?")]
        public async Task Introduce()
        {
            await ReplyAsync("Hello, I'm Iyoku and I'm here to help you create Collections\nWhat does that mean? You'll know soon!");
        }
    }
}
