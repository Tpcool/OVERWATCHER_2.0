using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot
{
    class DataStorage
    {
        public static Dictionary<string, string> pairs = new Dictionary<string, string>();
        private static readonly string path = @"..\..\..\SystemLang\storage.json";

        static DataStorage()
        {
            if (!ValidateStorageFile(path)) return;
            string json = File.ReadAllText(path);
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds a pair to the dictionary and saves to the file.
        /// </summary>
        public static void AddPairToStorage(string key, string value)
        {
            pairs.Add(key, value);
            SaveData();
        }
    }
}
