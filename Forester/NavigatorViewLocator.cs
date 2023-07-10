using Forester.ViewModels;
using Forester.ViewModels.App;
using Forester.ViewModels.App.Developer;
using Forester.ViewModels.Dialogs.SettingsDialog;
using Forester.Views;
using Forester.Views.App;
using Forester.Views.App.Developer;
using Forester.Views.Dialogs.SettingsDialog;
using ReactiveUI;
using System;

namespace Forester
{
    public class NavigatorViewLocator : IViewLocator
    {
        IViewFor IViewLocator.ResolveView<T>(T viewModel, string contract)
        {
            var name = viewModel!.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            return (IViewFor)Activator.CreateInstance(type!)!;
        }
    }
}
