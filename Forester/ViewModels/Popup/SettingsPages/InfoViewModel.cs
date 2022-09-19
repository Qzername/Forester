using Forester.Code;
using Forester.Code.AppData;
using Forester.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Popup.SettingsPages
{
    internal class InfoViewModel : ViewModelBase, IPage
    {
        public string Version { get => Data.config.version; }

        public void Discord()
        {
            Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/***REMOVED***", UseShellExecute = true });
        }

        public void PageClosed()
        {
        }

        public void PageOpened()
        {
        }

        public ViewModelBase ReceiveContent()
        {
            return this;
        }
    }
}
