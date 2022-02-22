using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Timers;

namespace Forester.Views
{
    public partial class LoginPanel : Window
    {
        
        public LoginPanel()
        {
            //From Theme.cs
            Data.ReadConfig();
            SetTheme();

            AvaloniaXamlLoader.Load(this);

            //From StyleButtons.cs
            isLogin = true;
            loginIsEnabled = true;
            loginOpacity = 1f;

            //From Waves.cs
            //Ustawienie zegaru do animacji
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 15); //co 15 ms nowa klatka co daje nam 1000/15= 66,(6) FPS
            timer.Start();
        }
    }
}
