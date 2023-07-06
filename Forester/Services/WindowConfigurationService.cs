using Forester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services
{
    public delegate void ChangeChromeHints(WindowConfiguration configuration);

    /// <summary>
    /// service for changing window configuration from diffrent views
    /// </summary>
    public class WindowConfigurationService
    {
        public event ChangeChromeHints OnChangeChromeHints;

        public void ChangeConfiguration(WindowConfiguration configuration)
        {
            OnChangeChromeHints?.Invoke(configuration);
        }
    }
}
