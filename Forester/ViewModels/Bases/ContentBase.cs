using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Bases
{
    public class ContentBase : ViewModelBase, IScreen
    {
        public RoutingState Router { get; }

        Dictionary<string, IRoutableViewModel> content;

        public ContentBase()
        {
            Router = new RoutingState();

            content = new Dictionary<string, IRoutableViewModel>();
        }

        public bool DoesExist(string name) => content.ContainsKey(name);

        public void AddView(string name, IRoutableViewModel viewModel)
        {
            content.Add(name, viewModel);
        }

        public void SwitchView(string name)
        {
            Router.Navigate.Execute(content[name]);
        }

        public void SwitchTemporaryView(IRoutableViewModel content)
        {
            Router.Navigate.Execute(content);
        }
    }
}
