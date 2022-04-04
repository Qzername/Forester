using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class DownloadUpdateViewModel
    {
        public SolidColorBrush first
        {
            get => MainWindowViewModel.first;
        }

        public SolidColorBrush second
        {
            get => MainWindowViewModel.second;
        }

        public SolidColorBrush third
        {
            get => MainWindowViewModel.third;
        }
    }
}
