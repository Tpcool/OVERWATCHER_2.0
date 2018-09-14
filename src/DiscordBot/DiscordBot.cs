using System.Threading.Tasks;
using DiscordBot.Connection;

namespace DiscordBot
{
    public class DiscordBot
    {
        private readonly IConnection connection;

        public DiscordBot(IConnection connection)
        {
            this.connection = connection;
        }

        public async Task Run()
        {
            await connection.Connect();
            await Task.Delay(-1);
        }
    }
}
