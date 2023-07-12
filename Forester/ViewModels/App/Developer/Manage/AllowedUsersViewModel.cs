using Avalonia.Collections;
using Forester.Data;
using Forester.Models.API;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.Manage
{
    internal class AllowedUsersViewModel : ChangePermissionBase
    {
        [Reactive] bool isPrivate { get; set; }

        public AllowedUsersViewModel(Application application, bool shouldBeEnabled) : base(Permission.Allowed, application, shouldBeEnabled)
        {
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

    }
}
