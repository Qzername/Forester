using Avalonia.Media;
using Forester.Models;
using ReactiveUI;
using Splat;
using System.ComponentModel;
using System.Globalization;

namespace Forester.ViewModels.Bases;

public class ViewModelBase : ReactiveObject
{
    protected T GetService<T>() => Locator.Current.GetService<T>()!;
}
