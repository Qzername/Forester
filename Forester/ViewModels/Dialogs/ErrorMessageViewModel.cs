using Forester.Models.Configurations;
using ReactiveUI.Fody.Helpers;

namespace Forester.ViewModels.Dialogs
{
    public class ErrorMessageViewModel : DialogBase
    {
        [Reactive] string statusCode { get; set; }
        [Reactive] string messageFriendly { get; set; }
        [Reactive] string messageTechnical { get; set; }
        
        public void SetError(ErrorMessage errorMessage)
        {
            statusCode = errorMessage.StatusCode == 0 ? string.Empty : errorMessage.ToString();
            messageFriendly = errorMessage.FriendlyMessage;
            messageTechnical = errorMessage.TechnicalMessage;
        }
    }
}
