using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Connection
{
    public class DiscordConnection : IConnection
    {
        private readonly DiscordSocketClient client;

        public DiscordConnection(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task Connect()
        {
            await client.LoginAsync(TokenType.Bot, "?");
            await client.StartAsync();
        }
    }
}
