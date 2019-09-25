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
            await InversionOfControl.Container.GetInstance<DiscordBot>().Run();
        }
    }
}
