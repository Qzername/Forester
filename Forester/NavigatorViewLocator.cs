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
