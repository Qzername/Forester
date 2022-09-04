using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.ManageApp
{
    internal class PrivateAccess : ViewModelBase
    {
        public static PrivateAccess Current;
        ObservableCollection<SingleAllowed> users { get; set; }
        bool _isPrivate;
        public bool isPrivate
        {
            get => _isPrivate;
            set
            {
                this.RaiseAndSetIfChanged(ref _isPrivate, value);
                IsCheckedHandle();
            }
        }

        string _errorPrivate;
        public string errorPrivate
        {
            get => _errorPrivate;
            set => this.RaiseAndSetIfChanged(ref _errorPrivate, value);
        }

        string _newUserPrivate;
        public string newUserPrivate
        {
            get => _newUserPrivate;
            set => this.RaiseAndSetIfChanged(ref _newUserPrivate, value);
        }

        public List<Account> allowedUsers;

        public PrivateAccess()
        {
            Current = this;

            users = new ObservableCollection<SingleAllowed>();
            allowedUsers = new List<Account>();
        }

        public void IsCheckedHandle()
        {
            if (ManageAppViewModel.Current.currentApp.isPrivate == isPrivate.ToString())
                return;

            Application update = new Application()
            {
                ID = 0,
                name = "",
                quickDescription = "",
                description = "",
                isPrivate = isPrivate.ToString(),
                mainDeveloper = 0,
                downloadNumber = 0,
                version = ""
            };

            ServerConnection.Put("/api/Applications/Update?name=" + ManageAppViewModel.Current.currentApp.name, update);
            ManageAppViewModel.Current.currentApp.isPrivate = isPrivate.ToString();
            DeveloperViewModel.Current.RefreshList();
        }

        public void AddUserPrivate()
        {
            errorPrivate = "";

            var username = newUserPrivate.Split('#');

            if (username.Length < 2)
            {
                errorPrivate = "Wrong username";
                return;
            }

            allowedUsers.Add(new Account()
            {
                ID = 0,
                friendlyUsername = username[0],
                friendly_ID = int.Parse(username[1]),
                description = "",
                isDeveloper = false,
                password = "",
                username = ""
            });

            Allowance.Current.UpdateDevelopersAllowed();
        }

        public void DeleteUserPrivate()
        {
            errorPrivate = "";

            var username = newUserPrivate.Split('#');

            if (!allowedUsers.Any(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])))
            {
                errorPrivate = "Wrong username";
                return;
            }

            allowedUsers.Remove(allowedUsers.Single(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])));

            Allowance.Current.UpdateDevelopersAllowed();
        }

        public void SetErrorMessage(string message)
        {
            errorPrivate = message;
        }

        public void SetAllowedUsers(bool isPrivateChange, Account[] allowedUsersList)
        {
            isPrivate = isPrivateChange;

            newUserPrivate = "";
            errorPrivate = "";

            for (int i = users.Count - 1; i > -1; i--)
                users.RemoveAt(i);

            allowedUsers.Clear();
            if (allowedUsersList is not null)
            {
                allowedUsers.AddRange(allowedUsersList);

                foreach (Account user in allowedUsersList)
                    users.Add(new SingleAllowed() { name = user.friendlyUsername + "#" + user.friendly_ID.ToString() });
            }
        }

        public void SwitchAllowedUser(string name) => newUserPrivate = name;
    }
}
