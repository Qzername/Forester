using Avalonia.Media.Imaging;
using Forester.Data;
using Forester.Models.API;
using Forester.Models.API.Pictures;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;

namespace Forester.Services
{
    public class UserDataService : ReactiveObject
    {
        [Reactive] public Account CurrentAccount { get; private set; }
        [Reactive] public string Login { get; private set; }
        [Reactive] public string Username { get; private set; }
        [Reactive] public Bitmap ProfilePicture { get; private set; }
        [Reactive] public Bitmap BackgroundPicture { get; private set; }

        //dependency injection
        AccountDatabase accountDatabase;
        PictureService pictureService;

        public UserDataService(AccountDatabase accountDatabase, PictureService pictureService)
        {
            this.accountDatabase = accountDatabase;
            this.pictureService = pictureService;
        }

        public async Task SetAccount(string login)
        {
            CurrentAccount = await accountDatabase.Get(login);

            Debug.Log("1");
            ProfilePicture = await pictureService.GetImage(login, ObjectType.Account, PictureType.ProfilePicture);
            Debug.Log("1");
            BackgroundPicture = await pictureService.GetImage(login, ObjectType.Account, PictureType.BackgroundPicture);
            Debug.Log(ProfilePicture.Size);
        }
    }
}
