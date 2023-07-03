using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Forester.Models;
using ReactiveUI.Fody.Helpers;

namespace Forester.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        [Reactive] Account account { get; set; }

        public LoginViewModel() 
        {
            account = new Account()
            {
                Login="test"
            };
        }

        public void TestFunc()
        {
            Debug.WriteLine("setset");
        }
    }
}
