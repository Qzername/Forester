using Avalonia.Controls;
using Forester.Models;
using Forester.Models.API;
using Forester.Models.App;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class SettingsFile : ReactiveObject
    {
        const string SettingsFilePath = "./Settings.json";

        //maybe in future change settings into abstract classes?

        [Reactive] public Settings Settings { get; private set; }
        public ForesterData ForesterData { get; private set; }

        public SettingsFile() => ReadSettings();

        public void ReadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                Settings = new Settings();

                SetDefaultData();
                SaveSettings();
            }
            else
                Settings = JsonManager.Deserialize<Settings>(File.ReadAllText(SettingsFilePath));

            ForesterData = JsonManager.Deserialize<ForesterData>(File.ReadAllText("./Version.json"));
        }

        public void SaveSettings() => File.WriteAllText(SettingsFilePath, JsonManager.Serialize(Settings));

        void SetDefaultData()
        {
            SetTheme("Spring");
            SetAutoLogin(new Account());
            SetLibraryElements(new LibraryElementConfig[0]);
        }

        /// <summary>
        /// Sets custom theme in settings, you have to save afterwards.
        /// Also removes "#" in hex code
        /// </summary>
        public void SetTheme(Theme theme)
        {
            Settings.Theme = new Theme()
            {
                Name = "Custom",
                FirstColor = theme.FirstColor.Trim('#'),
                SecondColor = theme.SecondColor.Trim('#'),
                ThirdColor = theme.ThirdColor.Trim('#')
            };
        }

        /// <summary>
        /// Sets default theme by name.
        /// Go to <see cref="Theme.DefaultThemes"/> to see default themes
        /// </summary>
        public void SetTheme(string name)
        {
            if (!Theme.DefaultThemes.Any(x => x.Name == name))
                throw new Exception($"Tried to set theme called \"{name}\" but that theme doesnt exist");

            Settings.Theme = Theme.DefaultThemes.Single(x => x.Name == name);
        }

        public void SetAutoLogin(Account account)
        {
            Settings.AutoLogin = new Account()
            {
                Login = account.Login,
                Username = account.Username,    
                Password = account.Password,
            };
        }

        public void SetLibraryElements(LibraryElementConfig[] libraryElements)
        {
            Settings.LibraryElements = libraryElements;
        }
    }
}
