using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot
{
    class DictionaryStorage
    {
        public static Dictionary<string, string> Pairs = new Dictionary<string, string>();
        private static readonly string Path = Constants.Storage;

        static DictionaryStorage()
        {
            if (!ValidateStorageFile(Path)) return;
            string json = File.ReadAllText(Path);
            Pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(Pairs, Formatting.Indented);
            File.WriteAllText(Path, json);
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
            Pairs.Add(key, value);
            SaveData();
        }
    }
}
