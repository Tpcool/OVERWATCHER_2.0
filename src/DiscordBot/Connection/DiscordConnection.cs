using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Configuration;

namespace DiscordBot.Connection
{
    public class DiscordConnection : IConnection
    {
        private readonly DiscordSocketClient client;
        private readonly IConfiguration config;

        public DiscordConnection(DiscordSocketClient client, IConfiguration config)
        {
            this.client = client;
            this.config = config;
        }

        public async Task Connect()
        {
            await client.LoginAsync(TokenType.Bot, config.GetValueFor(Constants.ConfigKeyToken));
            await client.StartAsync();
        }
    }
}
