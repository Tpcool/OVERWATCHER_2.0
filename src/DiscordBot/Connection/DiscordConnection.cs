using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Logging;
using DiscordBot.Core;
using System.Collections.Generic;
using DiscordBot.Utilities;
using System;

namespace DiscordBot.Connection
{
    public class DiscordConnection : IConnection
    {
        private readonly DiscordSocketClient client;
        private readonly IConfiguration config;
        private readonly ILogger logger;
        private readonly List<ulong> whitelist = DataStorage.LoadWhitelist(Constants.Whitelist);

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
            client.MessageReceived += OnMessageReceived;
            await client.SetGameAsync(Messages.GetAlert("System.Game"), type: ActivityType.Listening);
            await client.LoginAsync(TokenType.Bot, config.GetValueFor(Constants.ConfigKeyToken));
            await client.StartAsync();
            Global.Client = client;
            ConsoleTools.ConsoleInput();
        }

        private Task OnMessageReceived(SocketMessage msg)
        {
            if (IsValidUser(msg.Author))
            {
                var user = UserAccounts.GetAccount(msg.Author);
                user.AddCurrency(1);
            }
            if (Global.IsLoggingActive && !LogMessages.IsChannelBlacklisted(msg.Channel.Id))
            {
                LogMessages.AddOrAppendChannelLog(msg.Channel.Id, msg.Id);
                LogMessages.AppendLogToStorage(msg.Channel.Id, msg.Id);
            }
            return Task.CompletedTask;
        }

        private bool IsValidUser(SocketUser user)
        {
            if (user == null) return false; // User must exist
            if (!user.IsBot) return true; // Non bot users are always valid
            foreach (ulong bot in whitelist) // Check if there some bots that are allowed to be recognized
            {
                if (bot == user.Id) return true;
            }
            return false;
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
