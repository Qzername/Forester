using Avalonia.Collections;
using Forester.Data;
using Forester.Models.API;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.Manage
{
    public  class ChangePermissionBase :ViewModelBase
    {
        Permission permissionToChange;

        protected Application currentApplication;

        //dependency injection
        protected ApplicationDatabase applicationDatabase;
        protected DeveloperService developerService;

        protected AvaloniaList<Account> users { get; set; }
        [Reactive] protected string currentUser { get; set; }
        [Reactive] protected string error { get; set; }
        protected bool shouldBeEnabled { get; }

        public ChangePermissionBase(Permission permission, Application application, bool shouldBeEnabled)
        {
            this.shouldBeEnabled = shouldBeEnabled;

            permissionToChange = permission;
            currentApplication = application;

            applicationDatabase = GetService<ApplicationDatabase>();
            developerService = GetService<DeveloperService>();

            users = new AvaloniaList<Account>();
            _ = GetUsers();
        }

        protected async Task GetUsers()
        {
            users.Clear();

            if(permissionToChange == Permission.Allowed)
                users.AddRange(await applicationDatabase.GetApplicationAllowed(currentApplication));
            else
                users.AddRange(await applicationDatabase.GetApplicationDevelopers(currentApplication));
        }

        public void SwitchUser(object AccountOBJ)
        {
            Account account = (Account)AccountOBJ;

            currentUser = account.Login;
        }

        public async void AddUser()
        {
            error = "";

            if (users.Any(x => x.Login == currentUser))
            {
                error = "User already is " + permissionToChange.ToString();
                return;
            }

            bool isDone = await applicationDatabase.ChangePermission(currentApplication.Name, currentUser, permissionToChange);

            if (!isDone)
            {
                error = "User with that username doesnt exist";
                return;
            }

            currentUser = "";
            await GetUsers();
        }

        public async void DeleteUser()
        {
            error = "";

            if (!users.Any(x => x.Login == currentUser))
            {
                error = "User is not " + permissionToChange.ToString();
                return;
            }

            bool isDone = await applicationDatabase.ChangePermission(currentApplication.Name, currentUser, Permission.Remove);

            if (!isDone)
            {
                error = "User with that username doesnt exist";
                return;
            }

            currentUser = "";
            await GetUsers();
        }
    }
}
