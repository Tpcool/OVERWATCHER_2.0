using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.Utilities
{
    class Messages
    {
        protected static Dictionary<string, string> alerts;

        static Messages()
        {
            string path = Constants.Messages;
            string json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        /// <summary>
        /// Returns a message already formatted from a JSON file.
        /// </summary>
        /// <param name="key">Message to retrieve</param>
        /// <param name="param">Parameters for the message</param>
        /// <returns>Formatted message</returns>
        public static string GetAlert(string key, params object[] param)
        {
            if (alerts.ContainsKey(key)) return string.Format(alerts[key], param);
            return "";
        }

        /// <summary>
        /// Returns a message already formatted from a JSON file.
        /// </summary>
        /// <param name="key">Message to retrieve</param>
        /// <param name="param">Parameter for the message</param>
        /// <returns>Formatted message</returns>
        public static string GetAlert(string key, object param)
        {
            return GetAlert(key, new object[] { param });
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
