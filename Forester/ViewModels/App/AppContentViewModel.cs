using Forester.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class AppContentViewModel : ViewModelBase, IScreen
    {
        public RoutingState Router { get; }

        Dictionary<string, IRoutableViewModel> Content;

        public AppContentViewModel()
        {
            Router = new RoutingState();

            Content = new Dictionary<string, IRoutableViewModel>();
        }

        public bool DoesExist(string name) => Content.ContainsKey(name);

        public void AddView(string name, IRoutableViewModel viewModel)
        {
            Content.Add(name, viewModel);
        }

        public void SwitchView(string name)
        {
            Router.Navigate.Execute(Content[name]);
        }
    }
}
