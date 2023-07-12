using Avalonia.Collections;
using Forester.Data;
using Forester.Models.API;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;

namespace Forester.ViewModels.App.Developer.Manage
{
    internal class AllowedUsersViewModel : ViewModelBase
    {
        Application currentApplication;

        //dependency injection
        ApplicationDatabase applicationDatabase;
        DeveloperService developerService;

        [Reactive] bool isPrivate { get; set; }

        AvaloniaList<Account> users;

        [Reactive] string newUser { get; set; }
        [Reactive] string error { get; set; }

        public AllowedUsersViewModel(Application application)
        {
            currentApplication = application;

            applicationDatabase = GetService<ApplicationDatabase>();
            developerService = GetService<DeveloperService>();  

            isPrivate = currentApplication.IsPrivate;
        }

        public async void OnChangeIsPrivate()
        {
            await applicationDatabase.Update(currentApplication.Name, new Application()
            {
                IsPrivate = isPrivate
            });

            await developerService.RefreshApplicationList();

            currentApplication.IsPrivate = isPrivate;
        }

        public void SwitchUser(object AccountOBJ)
        {

        }

        public void AddUser()
        {

        }

        public void DeleteUser()
        {

        }
    }
}
