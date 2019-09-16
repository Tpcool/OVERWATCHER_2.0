using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Core
{
    public class Functions
    {
        public enum BotEmbedColor { Light, Dark };

        public static bool UserHasRole(SocketGuildUser user, string roleName)
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

        public static EmbedBuilder GetDefaultBotEmbed(BotEmbedColor color = BotEmbedColor.Light)
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

        /// <summary>
        /// Finds and returns a user with the most similar name to the string passed through.
        /// Returns a null user if there are no matches.
        /// </summary>
        public static SocketUser GetUserWithSimilarName(string testName)
        {
            var accounts = UserAccounts.GetAccounts();
            testName = testName.ToLower();

            var result = // Retrieves the SocketUsers with similar usernames to the passed name
                from acc in accounts
                where Global.GetSocketUserWithId(acc.Id).Username.ToLower().Contains(testName)
                select Global.GetSocketUserWithId(acc.Id);

            SocketUser closestName = null;
            foreach (SocketUser u in result)
            {
                if (u.Username.ToLower().Equals(testName)) // Return if there is an equal match
                {
                    return u;
                }
                else if (u.Username.ToLower().Contains(testName)) // Otherwise, store the user with closest matching name
                {
                    closestName = u;
                }
            }
            return closestName;
        }
    }
}
