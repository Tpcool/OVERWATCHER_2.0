using Discord;

namespace DiscordBot
{
    public static class Constants
    {
        public const string ConfigKeyToken = "DiscordToken";
        public const string CmdPrefix = "CommandPrefix";
        public const string Whitelist = @"..\..\..\SystemLang\whitelist.json";
        public const string Accounts = @"..\..\..\SystemLang\accounts.json";
        public const string Messages = @"..\..\..\SystemLang\messages.json";
        public const string Storage = @"..\..\..\SystemLang\storage.json";
        public static Color LightOver = new Color(170, 142, 214);
        public static Color DarkOver = new Color(85, 57, 134);
        public static Color LightKirby = new Color(243, 165, 170);
        public static Color DarkKirby = new Color(224, 0, 91);
    }
}
