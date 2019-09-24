using Discord;
using Discord.Commands;
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
                Console.WriteLine($"Cannot find role {roleName} in channel.");
                return false;
            }
            var targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }

        public static EmbedBuilder GetDefaultBotEmbed(BotEmbedColor color = BotEmbedColor.Light)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(FormatMessages.GetFormattedAlert("system.defaultEmbed"));
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
            if (testName.Trim().Equals(string.Empty)) return null;

            var accounts = UserAccounts.GetAccounts();
            testName = testName.ToLower();

            // Retrieves the SocketUsers with similar usernames to the passed name
            var result = from acc in accounts
                         where Global.GetSocketUserWithId(acc.Id).Username.ToLower().Contains(testName)
                         select Global.GetSocketUserWithId(acc.Id);
            
            List<string> resultUsernames = new List<string>();
            List<ulong> resultIds = new List<ulong>();

            foreach (SocketUser u in result)
            {
                resultUsernames.Add(u.Username);
                resultIds.Add(u.Id);
            }

            if (resultIds.Count == 0) return null;
            ulong idMatch = GetMatch(resultIds, resultUsernames, testName);
            SocketUser closestName = Global.GetSocketUserWithId(idMatch);
            return closestName;
        }

        private static ulong GetMatch(List<ulong> ids, List<string> usernames, string testName)
        {
            int charDifference = int.MaxValue;
            int tempCharDifference = 0;
            int indexOfMatch = 0;
            testName = testName.ToLower();

            for (int i = 0; i < ids.Count(); i++)
            {
                string username = usernames.ElementAt(i).ToLower();
                tempCharDifference = username.Length - testName.Length;

                if (username.Equals(testName)) // Return if there is an equal match
                {
                    return ids.ElementAt(i);
                }
                else if (username.Contains(testName) && tempCharDifference < charDifference) // Otherwise, store the user with closest matching name
                {
                    indexOfMatch = i;
                    charDifference = tempCharDifference;
                }
            }
            return ids.ElementAt(indexOfMatch);
        }

        public static bool HasMentionedUsers(SocketUserMessage msg)
        {
            return (msg.MentionedUsers.Count == 0 && msg.MentionedRoles.Count == 0) ? false : true;
        }

        public struct HelpEntry
        {
            public string name;
            public string category;
            public string parameters;
            public string description;
        }

        /// <summary>
        /// Returns a list of dictionaries with the category of the command as a key and the command itself as the value.
        /// </summary>
        public static List<HelpEntry> GetHelpList(CommandService cmd)
        {
            var helpEntries = new List<HelpEntry>();

            foreach(var command in cmd.Commands)
            {
                string paramDescription = "";
                foreach(var param in command.Parameters)
                {
                    if (param.Name.Equals("nothing")) break; // Break if the parameter exists just to allow users to type after the command without consequence.
                    paramDescription += $"[{param.Name}]";
                    if (param.IsOptional) paramDescription += "*";
                    paramDescription += ", ";
                }
                paramDescription = paramDescription.TrimEnd(' ').TrimEnd(',');

                HelpEntry entry = new HelpEntry
                {
                    name = command.Name,
                    category = command.Remarks,
                    parameters = paramDescription,
                    description = command.Summary
                };
                helpEntries.Add(entry);
            }
            return helpEntries;
        }
    }
}
