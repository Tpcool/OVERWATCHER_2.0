using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace DiscordBot.Core
{
    public class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string path)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        
        public static IEnumerable<UserAccount> LoadUserAccounts(string path)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static List<ulong> LoadWhitelist(string path)
        {
            if (!File.Exists(path)) return null;
            var whitelist = new List<ulong>();
            var list = File.ReadLines(path);

            foreach (string f in list)
            {
                if (ulong.TryParse(f, out ulong id)) whitelist.Add(id);
            }
            return whitelist;
        }

        public static bool SaveExists(string path)
        {
            return File.Exists(path);
        }
    }
}
