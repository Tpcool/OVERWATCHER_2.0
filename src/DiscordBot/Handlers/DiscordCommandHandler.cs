using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Logging;

namespace DiscordBot.Handlers
{
    public class DiscordCommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commandService;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        public DiscordCommandHandler(DiscordSocketClient client, CommandService commandService, ILogger logger, IConfiguration config)
        {
            this.client = client;
            this.commandService = commandService;
            this.logger = logger;
            this.config = config;
        }

        public async Task InitializeAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            
            var argPos = 0;
            if (msg.HasStringPrefix(config.GetValueFor(Constants.CmdPrefix), ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(client, msg);
                await TryRunAsBotCommand(context, argPos).ConfigureAwait(false);
            }
        }

        private async Task TryRunAsBotCommand(SocketCommandContext context, int argPos)
        {
            var result = await commandService.ExecuteAsync(context, argPos, InversionOfControl.Container);

            if(!result.IsSuccess)
            {
                logger.Log($"Command execution failed. Reason: {result.ErrorReason}.");
            }
        }
    }
}
