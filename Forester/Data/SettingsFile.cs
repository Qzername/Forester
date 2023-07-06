using Avalonia.Controls;
using Forester.Models;
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
        }

        public void SaveSettings() => File.WriteAllText(SettingsFilePath, JsonManager.Serialize(Settings));

        void SetDefaultData()
        {
            SetTheme("Spring");
            SetAutoLogin(new Account());
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
    }
}
