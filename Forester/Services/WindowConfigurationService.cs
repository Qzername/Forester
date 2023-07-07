using Forester.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services
{
    /// <summary>
    /// service for changing window configuration from diffrent views
    /// </summary>
    public class WindowConfigurationService
    {
        public delegate void ChangeWindowConfiguration(WindowConfiguration configuration);

        public event ChangeWindowConfiguration OnChangeConfiguration;
        public event Action OnLogOutCalled;

        public void ChangeConfiguration(WindowConfiguration configuration)
        {
            OnChangeConfiguration?.Invoke(configuration);
        }

        public void LogOut()
        {
            OnLogOutCalled?.Invoke();
        }
    }
}
