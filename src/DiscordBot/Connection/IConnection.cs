using System.Threading.Tasks;

namespace DiscordBot
{
    public interface IConnection
    {
        Task Connect();
    }
}
