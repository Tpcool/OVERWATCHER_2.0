using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Connection
{
    class ConsoleTools
    {
        public static async Task ConsoleInput()
        {
            string input = "";
            while (input != "")
            {
                Console.WriteLine("Awaiting console command.");
                input = Console.ReadLine().ToLower().Trim();
            }
        }
    }
}
