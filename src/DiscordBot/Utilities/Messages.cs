using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.Utilities
{
    class Messages
    {
        protected static Dictionary<string, string> alerts;

        static Messages()
        {
            string path = @"..\..\..\SystemLang\messages.json";
            string json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static void DisplayAlerts()
        {
            foreach (KeyValuePair<string, string> val in alerts)
            {
                Console.WriteLine($"{val.Key} - {val.Value}");
            }
        }
    }
}
