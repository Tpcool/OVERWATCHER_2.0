using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _cmd;

        public GeneralCommands(CommandService cmd)
        {
            _cmd = cmd;
        }

        [Command("help"), Remarks("general"),
            Summary("Displays the list of commands, or the use of a specific command.")]
        public async Task Help([Remainder]string command = "")
        {
            StringBuilder commandList = new StringBuilder(Constants.CharacterLimit);
            var commands = Functions.GetHelpList(_cmd);
            command = command.ToLower().Trim().Trim('.'); // todo: replace hardcoded command prefix

            if (!command.Equals("")) //todo: condense ifelse branch?
            {
                foreach (var entry in commands)
                {
                    if (entry.name.Equals(command))
                    {
                        commandList.Append($"`.{entry.name} {entry.parameters}`, {entry.category} command\n{entry.description}");
                        break;
                    }
                }
                if (commandList.Equals(""))
                {
                    commandList.Append($"The `.{command}` command could not be found.");
                }
            }
            else
            {
                string previousCategory = "";
                commands = commands.OrderBy(c => c.category).ToList();
                foreach (var entry in commands)
                {
                    if (!previousCategory.Equals(entry.category)) commandList.Append($"\n\n{entry.category} commands:\n");
                    commandList.Append($"`.{entry.name}` ");
                    previousCategory = entry.category;
                }
                commandList.Append($"\n\nEnter `.help [command]*` for more info. 😈 Optional parameters are marked with an asterisk.");
            }

            await Context.Channel.SendMessageAsync(commandList.ToString());
        }

    }
}
