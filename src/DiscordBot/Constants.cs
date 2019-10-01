using Discord;

namespace DiscordBot
{
    public static class Constants
    {
        public const string ConfigKeyToken = "DiscordToken";
        public const string Whitelist = @"..\..\..\..\DiscordBot\SystemLang\whitelist.txt";
        public const string Accounts = @"..\..\..\..\DiscordBot\SystemLang\accounts.json";
        public const string Messages = @"..\..\..\..\DiscordBot\SystemLang\messages.json";
        public const string Storage = @"..\..\..\..\DiscordBot\SystemLang\storage.json";
        public const string LogBlacklist = @"..\..\..\..\DiscordBot\SystemLang\logblacklist.txt";
        public const string LogDirectory = @"..\..\..\..\DiscordBot\ServerLogs\";
        public const int CharacterLimit = 2000;
        public static readonly Color LightOver = new Color(170, 142, 214);
        public static readonly Color DarkOver = new Color(85, 57, 134);
        public static readonly Color LightKirby = new Color(243, 165, 170);
        public static readonly Color DarkKirby = new Color(224, 0, 91);
    }
}
