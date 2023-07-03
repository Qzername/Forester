using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Forester.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Forester.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        [Reactive] string login { get; set; }
        [Reactive] string username { get; set; }
        [Reactive] string password { get; set; }

        public void ResetData()
        {
            login = string.Empty;
            username = string.Empty;
            password = string.Empty;
        }
    }
}
