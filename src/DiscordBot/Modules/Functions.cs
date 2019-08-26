using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Modules
{
    class Functions
    {
        public enum BotEmbedColor { Light, Dark };

        static public bool UserHasRole(SocketGuildUser user, string roleName)
        {
            var result = from r in user.Guild.Roles
                         where r.Name == roleName
                         select r.Id;
            ulong roleId = result.FirstOrDefault();
            if (roleId == 0)
            {
                Console.WriteLine(Utilities.FormatMessages.GetFormattedAlert("ErrorRole_&ROLE", roleName));
                return false;
            }
            var targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }

        static public EmbedBuilder GetDefaultBotEmbed(BotEmbedColor color = BotEmbedColor.Light)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(FormatMessages.GetFormattedAlert("DefaultTitle"));
            switch (color)
            {
                case BotEmbedColor.Light:
                    embed.WithColor(Constants.LightOver); break;
                case BotEmbedColor.Dark:
                    embed.WithColor(Constants.DarkOver); break;
            }

            return embed;
        }
    }
}
