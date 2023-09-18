using Forester.Models.Configurations;
using Forester.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services
{
    public class ErrorMessageService
    {
        //some lazy loading would be cool here
        ErrorMessageViewModel errorMessage { get; set; }
        DialogConfiguration defaultConfiguration { get; set; }

        //dependency injection
        DialogService dialogService { get; set; }

        public ErrorMessageService(DialogService dialogService)
        {
            this.dialogService = dialogService;
            errorMessage = new ErrorMessageViewModel();

            defaultConfiguration = new DialogConfiguration()
            {
                Content = errorMessage,
                Width = 500,
                Height = 400,
            }; 
        }

        public void SendErrorMessage(ErrorMessage message)
        {
            errorMessage.SetError(message);

            dialogService.ChangeConfiguration(defaultConfiguration);
            dialogService.ChangeVisibility(true);
        }
    }
}
