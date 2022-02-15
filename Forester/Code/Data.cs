using Avalonia.Media;
using Forester.Models;
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

        /// <summary>
        /// Read config
        /// </summary>
        public static void ReadConfig() => config = JsonConverter.Deserialize<Config>(FileReader.ReadText("./config.json"));

        /// <summary>
        /// Save config
        /// </summary>
        public static void SaveConfig(Config config) => FileReader.SaveText("./config.json", JsonConverter.Serialize(config));
    }

    public struct ThemeColor
    {
        public string name;
        public Color first;
        public Color second;
        public Color third;
    }
}
