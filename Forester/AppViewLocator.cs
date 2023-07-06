using Forester.ViewModels;
using Forester.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    public class AppViewLocator : IViewLocator
    {
        IViewFor IViewLocator.ResolveView<T>(T viewModel, string contract)
        {
            switch (viewModel)
            {
                case LoginViewModel context:
                    return new LoginView()
                    {
                        DataContext = context,
                    };
                case AppViewModel context:
                    return new AppView()
                    {
                        DataContext = context
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewModel));
            }
        }
    }
}
