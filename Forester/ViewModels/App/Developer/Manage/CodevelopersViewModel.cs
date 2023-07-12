using Forester.Models.API;
using Forester.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.Manage
{
    internal class CodevelopersViewModel : ViewModelBase
    {
        Application currentApplication;

        public CodevelopersViewModel(Application application)
        {
            currentApplication = application;
        }
    }
}
