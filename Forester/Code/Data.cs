using Avalonia.Media;
using Forester.Code;
using Forester.Models;
using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    /// <summary>
    /// Class that contains various data i.e. from config
    /// </summary>
    public static class Data
    {
        public static Config config;
        public static Token token;
        public static Account account;

        /// <summary>
        /// Read config
        /// </summary>
        public static void ReadConfig() => config = JsonConverter.Deserialize<Config>(FileReader.ReadText("./dataConfig.json"));

        /// <summary>
        /// Save config
        /// </summary>
        public static void SaveConfig(Config config) 
        {
            Data.config = config;
            FileReader.SaveText("./dataConfig.json", JsonConverter.Serialize(config)); 
        }
    }

    public struct ThemeColor
    {
        public string name;
        public Color first;
        public Color second;
        public Color third;
    }
}
