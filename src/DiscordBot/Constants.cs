using Discord;

namespace DiscordBot
{
    public static class Constants
    {
        public const string ConfigKeyToken = "DiscordToken";
        public const string CmdPrefix = "CommandPrefix";
        public const string Whitelist = @"..\..\..\..\DiscordBot\SystemLang\whitelist.txt";
        public const string Accounts = @"..\..\..\..\DiscordBot\SystemLang\accounts.json";
        public const string Messages = @"..\..\..\..\DiscordBot\SystemLang\messages.json";
        public const string Storage = @"..\..\..\..\DiscordBot\SystemLang\storage.json";
        public static readonly Color LightOver = new Color(170, 142, 214);
        public static readonly Color DarkOver = new Color(85, 57, 134);
        public static readonly Color LightKirby = new Color(243, 165, 170);
        public static readonly Color DarkKirby = new Color(224, 0, 91);
    }
}
