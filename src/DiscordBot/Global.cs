using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public static class Global
    {
        public static bool IsLoggingActive = false;
        internal static DiscordSocketClient Client { get; set; }
        internal static ulong MessageIdToTrack { get; set; } // Single message, could use list for multiple

        public static SocketUser GetSocketUserWithId(ulong id) => Client.GetUser(id);
        public static SocketGuild GetSocketGuildWithId(ulong id) => Client.GetGuild(id);
        public static SocketChannel GetSocketChannelWithId(ulong id) => Client.GetChannel(id);
        public static SocketGuild GetSocketGuildWithChannelId(ulong id)
        {
            SocketTextChannel a = (SocketTextChannel)Client.GetChannel(id);
            return a.Guild;
        }
        public static bool IsValidChannelId(ulong id)
        {
            IChannel channel = GetSocketChannelWithId(id);
            if (channel == null) return false;
            return true;
        }
    }
}
