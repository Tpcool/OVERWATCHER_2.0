using System.Threading.Tasks;
using DiscordBot.Connection;
using DiscordBot.Handlers;

namespace DiscordBot
{
    public class DiscordBot
    {
        private readonly IConnection connection;
        private readonly ICommandHandler commandHandler;

        public DiscordBot(IConnection connection, ICommandHandler commandHandler)
        {
            this.connection = connection;
            this.commandHandler = commandHandler;
        }

        public async Task Run()
        {
            await connection.Connect();
            await commandHandler.InitializeAsync();
            await Task.Delay(-1);
        }
    }
}
