using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // https://discordapp.com/api/oauth2/authorize?client_id=462346036594212874&scope=bot&permissions=8
            await InversionOfControl.Container.GetInstance<DiscordBot>().Run();
        }
    }
}
