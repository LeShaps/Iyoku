using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iyoku.Extensions
{
    public static class SocketGuildExtensions
    {
        public static List<SocketGuildChannel> GetChannelsOfCategory(this SocketGuild Guild, string Category)
            => Guild.CategoryChannels.Where(category => category.Name == Category).FirstOrDefault().Channels.ToList();

        public static bool IsDm(this SocketMessage Message)
            => Message.Channel as SocketGuildChannel is null;
    }
}
