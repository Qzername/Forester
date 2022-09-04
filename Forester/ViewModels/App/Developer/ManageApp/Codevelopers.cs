using Forester.Models.API;
using Forester.Views.App.Developer.ManageApp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.ManageApp
{
    internal class Codevelopers : ViewModelBase
    {
        public static Codevelopers Current;

        string _errorDeveloper;
        public string errorDeveloper
        {
            get => _errorDeveloper;
            set => this.RaiseAndSetIfChanged(ref _errorDeveloper, value);
        }

        string _newDeveloper;
        public string newDeveloper
        {
            get => _newDeveloper;
            set => this.RaiseAndSetIfChanged(ref _newDeveloper, value);
        }
        ObservableCollection<SingleAllowed> allowedDevelopers { get; set; }
        public List<Account> developers;

        public Codevelopers()
        {
            Current = this;

            allowedDevelopers = new ObservableCollection<SingleAllowed>();
            developers = new List<Account>();
        }

        public void AddDeveloper()
        {
            errorDeveloper = "";

            var username = newDeveloper.Split('#');

            if (username.Length < 2)
            {
                errorDeveloper = "Wrong username";
                return;
            }

            developers.Add(new Account()
            {
                ID = 0,
                friendlyUsername = username[0],
                friendly_ID = int.Parse(username[1]),
                description = "",
                isDeveloper = false,
                password = "",
                username = ""
            });

            Allowance.Current.UpdateDevelopersAllowed(true);
        }

        public void DeleteDeveloper()
        {
            errorDeveloper = "";
            var username = newDeveloper.Split('#');

            if (!developers.Any(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])))
            {
                errorDeveloper = "Wrong username";
                return;
            }
            developers.Remove(developers.Single(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])));
            Allowance.Current.UpdateDevelopersAllowed(true);
        }

        public void SetErrorMessage(string message)
        {
            errorDeveloper = message;
        }

        public void SetAllowedDevelopers(Account[] allowedDevelopersList)
        {
            newDeveloper = "";
            errorDeveloper = "";

            for (int i = allowedDevelopers.Count - 1; i > -1; i--)
                allowedDevelopers.RemoveAt(i);

            developers.Clear();
            if (allowedDevelopersList is not null)
            {
                developers.AddRange(allowedDevelopersList);

                foreach (Account user in allowedDevelopersList)
                    allowedDevelopers.Add(new SingleAllowed() { name = user.friendlyUsername + "#" + user.friendly_ID.ToString() });
            }
        }

        public void SwitchDeveloper(string name) => newDeveloper = name;
    }
}
