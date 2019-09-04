using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Logging;
using DiscordBot.Core;

namespace DiscordBot.Connection
{
    public class DiscordConnection : IConnection
    {
        private readonly DiscordSocketClient client;
        private readonly IConfiguration config;
        private readonly ILogger logger;

        public DiscordConnection(DiscordSocketClient client, IConfiguration config, ILogger logger)
        {
            this.client = client;
            this.config = config;
            this.logger = logger;
        }

        public async Task Connect()
        {
            client.Log += logger.Log;
            client.Ready += RepeatingTimer.StartTimer;
            client.ReactionAdded += OnReactAdded;
            await client.LoginAsync(TokenType.Bot, config.GetValueFor(Constants.ConfigKeyToken));
            await client.StartAsync();
            Global.Client = client;
        }

        private async Task OnReactAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if(reaction.MessageId == Global.MessageIdToTrack)
            {
                if (reaction.Emote.Name == "😈") {
                    await channel.SendMessageAsync(reaction.User.Value.Username + " reacted with Overwatcher.");
                }
            }
        }
    }
}
