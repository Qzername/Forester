using Forester.Models.Configurations;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class ErrorMessageViewModel : DialogBase
    {
        [Reactive] string statusCode { get; set; }
        [Reactive] string messageFriendly { get; set; }
        [Reactive] string messageTechnical { get; set; }
        
        public void SetError(ErrorMessage errorMessage)
        {
            statusCode = errorMessage.StatusCode.ToString();
            messageFriendly = errorMessage.FriendlyMessage;
            messageTechnical = errorMessage.TechnicalMessage;
        }
    }
}
