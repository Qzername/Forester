using Forester.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services
{

    public class DialogService
    {
        public delegate void ChangeDialogConfiguration(DialogConfiguration configuration);
        public delegate void SwitchDialogVisibility(bool isVisible);

        public event ChangeDialogConfiguration OnConfigurationChange;
        public event SwitchDialogVisibility OnVisibilityChange;

        public void ChangeConfiguration(DialogConfiguration configuration)
        {
            OnConfigurationChange?.Invoke(configuration);
        }

        public void ChangeVisibility(bool isVisible)
        {
            OnVisibilityChange?.Invoke(isVisible);
        }
    }
}
