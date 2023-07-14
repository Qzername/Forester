using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Forester.Controls;
using Forester.Data;
using Forester.Models.API;
using Forester.Models.API.Pictures;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;
using System;
using System.Threading.Tasks;
using Ava = Avalonia;

namespace Forester.ViewModels.App.Developer.Manage
{
    internal class BasicInformationViewModel : ViewModelBase
    {
        Application currentApplication { get; set; }

        //dependency injection
        UserDataService userDataService;
        ApplicationDatabase applicationDatabase;
        PictureService pictureService;
        DeveloperService developerService;
        DialogService dialogService;

        //pictures
        [Reactive] Bitmap profilePicture { get; set; }
        [Reactive] Bitmap backgroundPicture { get; set; }

        //base data
        [Reactive] string Name { get; set; }
        [Reactive] string ShortDescription { get; set; }
        [Reactive] string Description { get; set;}

        [Reactive] string error { get; set; }

        //make changes button
        [Reactive] bool makeChanges { get; set; }
        [Reactive] string buttonText { get; set; }

        protected bool shouldBeEnabled { get; }

        public BasicInformationViewModel(Application application, bool shouldBeEnabled)
        {
            this.shouldBeEnabled = shouldBeEnabled;

            makeChanges = false;
            buttonText = "Make changes";

            currentApplication = application;

            userDataService = GetService<UserDataService>();
            applicationDatabase = GetService<ApplicationDatabase>();    
            pictureService = GetService<PictureService>();
            developerService = GetService<DeveloperService>();
            dialogService = GetService<DialogService>();

            _ = SetDefaultData();
        }

        async Task SetDefaultData()
        {
            Name = currentApplication.Name;
            ShortDescription = currentApplication.ShortDescription;
            Description = currentApplication.Description;

            await SetPictures();
        }

        async Task SetPictures()
        {
            profilePicture = await pictureService.GetImage(currentApplication.Name, ObjectType.Application, PictureType.ProfilePicture);
            backgroundPicture = await pictureService.GetImage(currentApplication.Name, ObjectType.Application, PictureType.BackgroundPicture);
        }

        public void SwitchChanges()
        {
            makeChanges = !makeChanges;

            if (makeChanges)
                buttonText = "Cancel";
            else
            {
                error = string.Empty;
                _ = SetDefaultData();
                buttonText = "Make changes";
            }
        }

        public async void UploadNewProfilePicture() => await UpdatePicture(PictureType.ProfilePicture);
        public async void UploadNewBackgroundPicture() => await UpdatePicture(PictureType.BackgroundPicture);

        async Task UpdatePicture(PictureType pictureType)
        {
            string file = await dialogService.OpenFileDialog(new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[]
                    {
                        FilePickerFileTypes.ImageAll
                    }
            });

            if (file == string.Empty)
                return;

            await pictureService.UpdateImage(new Bitmap(file), currentApplication.Name, ObjectType.Application, pictureType);
            await SetPictures();
        }

        public async void SubmitChanges()
        {
            if (string.IsNullOrEmpty(Name))
            {
                error = "Name has to be fullfield";
                return;
            }

            var isDone = await applicationDatabase.Update(currentApplication.Name,
                new Application()
                {
                    Name = currentApplication.Name == Name ? string.Empty : Name,
                    ShortDescription = ShortDescription,
                    Description = Description,
                });

            if (!isDone)
                return;

            await developerService.RefreshApplicationList();
            developerService.MoveToApp(Name);
        }
    }
}
