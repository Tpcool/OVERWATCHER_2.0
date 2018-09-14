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
