using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordBot.Core
{
    internal static class RepeatingTimer
    {
        private static Timer loopingTimer;
        public static SocketTextChannel Channel { get; }

        internal static Task StartTimer()
        {
            if (!IsClientReady()) return Task.CompletedTask;

            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = false
            };
            loopingTimer.Elapsed += OnTimerTicked;

            Console.WriteLine("Start timer...");
            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            // await channel.SendMessageAsync("");
        }

        private static bool IsClientReady()
        {
            if (Global.Client== null)
            {
                Console.WriteLine("Timer ticked before client was ready.");
                return false;
            }
            return true;
        }
    }
}
