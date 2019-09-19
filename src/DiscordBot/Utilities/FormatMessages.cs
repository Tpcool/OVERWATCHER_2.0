using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities
{
    class FormatMessages : Messages
    {
        /// <summary>
        /// Returns a message already formatted from a JSON file.
        /// </summary>
        /// <param name="key">Message to retrieve</param>
        /// <param name="param">Parameters for the message</param>
        /// <returns>Formatted message</returns>
        public static string GetFormattedAlert(string key, params object[] param)
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
        public static string GetFormattedAlert(string key, object param)
        {
            return GetFormattedAlert(key, new object[] { param });
        }
    }
}
