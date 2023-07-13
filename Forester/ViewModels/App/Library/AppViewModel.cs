using Forester.Models.API;
using Forester.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
        Application currentApplication;
        
        public AppViewModel(Application application)
        {
            currentApplication = application;
        }
    }
}
