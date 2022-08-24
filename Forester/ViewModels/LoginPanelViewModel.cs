using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class LoginPanelViewModel : ViewModelBase
    {
        public string login;
        public string username;
        public string password;
        public string error;
        public bool rememberMe;

        public string Login
        {
            get => login;
            set => this.RaiseAndSetIfChanged(ref login, value);
        }

        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        public string Error
        {
            get => error;
            set => this.RaiseAndSetIfChanged(ref error, value);
        }

        public bool RememberMe
        {
            get => rememberMe;
            set => this.RaiseAndSetIfChanged(ref rememberMe, value);
        }

        bool isLogin;

        bool loginIsEnabled, registerIsEnabled;
        float loginOpacity, registerOpacity;

        public bool LoginIsEnabled
        {
            get => loginIsEnabled;
            set => this.RaiseAndSetIfChanged(ref loginIsEnabled, value);
        }

        public bool RegisterIsEnabled
        {
            get => registerIsEnabled;
            set => this.RaiseAndSetIfChanged(ref registerIsEnabled, value);
        }

        public float LoginOpacity
        {
            get => loginOpacity;
            set => this.RaiseAndSetIfChanged(ref loginOpacity, value);
        }

        public float RegisterOpacity
        {
            get => registerOpacity;
            set => this.RaiseAndSetIfChanged(ref registerOpacity, value);
        }

        public Geometry wave1, wave2, wave3;
        public Geometry Wave1
        {
            get => wave1;
            set => this.RaiseAndSetIfChanged(ref wave1, value);
        }
        public Geometry Wave2
        {
            get => wave2;
            set => this.RaiseAndSetIfChanged(ref wave2, value);
        }
        public Geometry Wave3
        {
            get => wave3;
            set => this.RaiseAndSetIfChanged(ref wave3, value);
        }

        DispatcherTimer timer;

        double currentOffset = 0;


        public LoginPanelViewModel()
        {
            MainWindowViewModel.Current.ToolBarHeight = 50;

            isLogin = true;
            LoginIsEnabled = true;
            LoginOpacity = 1f;

            //Ustawienie zegaru do animacji
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 15); //co 15 ms nowa klatka co daje nam 1000/15= 66,(6) FPS
            timer.Start();
        }

        //Klatka animacji wszystkie fale
        void Timer_Tick(object? sender, EventArgs e)
        {
            currentOffset += 5;

            Geometry tempWave1 = Wave1;
            Geometry tempWave2 = Wave2;
            Geometry tempWave3 = Wave3;

            //Offsety są już ustawione
            GeneratePath(new Point(0 + currentOffset * 0.3, MainWindowViewModel.Current.Height - 420), 500, 150, ref tempWave1, false);
            GeneratePath(new Point(0 + currentOffset * 0.6, MainWindowViewModel.Current.Height - 250), 600, 100, ref tempWave2, true);
            GeneratePath(new Point(400 + currentOffset, MainWindowViewModel.Current.Height - 100), 800, 50, ref tempWave3, false);

            Wave1 = tempWave1;
            Wave2 = tempWave2;
            Wave3 = tempWave3;
        }

        /// <summary>
        /// Generacja klatki animacji jednej ścieżki (fali)
        /// </summary>
        /// <param name="offset">Offset od którego ma być generowana mapa (liczy się od prawego rogu)</param>
        /// <param name="width">szerokość fali</param>
        /// <param name="height">wysokość fali</param>
        /// <param name="objectName">nazwa obiektu do którego ma zostać przypisana fala (musi byc ustawiona atrybutem Name="")</param>
        /// <param name="startUpside">czy ma zostać odwrócona, zaczynać od dołu, a nie od wzgórza</param>
        void GeneratePath(Point offset, double width, double height, ref Geometry geometry, bool startUpside)
        {
            PathGeometry pathGeometry = new PathGeometry();

            //W związku z tym że render dla ułatwienia animacji rysowana jest od prawego rogu okna,
            //Do starting point dodaje całą szerokośc okna
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(MainWindowViewModel.Current.Width + offset.X, offset.Y);
            pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);

            //Segmenty fali, rysowanie na całej płaszczyźnie ekranu
            for (int i = 0; i < (MainWindowViewModel.Current.Width + offset.X) / width; i++)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point3 = new Point(MainWindowViewModel.Current.Width - width * (i + 1) + offset.X, offset.Y);
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width / 2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(MainWindowViewModel.Current.Width - width * i + offset.X, offset.Y);

                pathFigure.Segments.Add(bezierSegment);
            }

            //Łączenie ostatniego segmentu z lewym dolnym rogiem okna
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, MainWindowViewModel.Current.Height);
            pathFigure.Segments.Add(lineSegment);

            //Następnie przeciągniecie linii do prawego dolnego
            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = new Point(MainWindowViewModel.Current.Width, MainWindowViewModel.Current.Height);
            pathFigure.Segments.Add(lineSegment2);

            //i na koniec do punktu startowego
            LineSegment lineSegment3 = new LineSegment();
            lineSegment3.Point = new Point(MainWindowViewModel.Current.Width + offset.X, offset.Y);
            pathFigure.Segments.Add(lineSegment3);

            Path path = new Path();
            path.Stretch = Stretch.None;
            path.Data = pathGeometry;

            geometry = path.Data;
        }

        public void SwitchPanels(Button button)
        {
            isLogin = !isLogin;

            LoginIsEnabled = isLogin;
            LoginOpacity = isLogin ? 1 : 0;

            RegisterIsEnabled = !isLogin;
            RegisterOpacity = isLogin ? 0 : 1;

            button.Content = isLogin ? "Don't have an account?" : "Already have an account?";

            Login = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Error = string.Empty;
        }

        public void Exit(Window window) => window.Close();

        public void LoginIntoAccount()
        {
            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
            {
                Error = "Login and password cannot be empty.";
                return;
            }

            LoginCredentials loginC = new LoginCredentials()
            {
                username = Login,
                password = Password
            };

            var response = ServerConnection.Post("/api/Accounts/Login", loginC);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //zalogowano
                //remember me
                if (RememberMe)
                {
                    var config = Data.config;
                    config.autoLogin = Login;
                    config.autoPassword = Password;
                    Data.SaveConfig(config);
                }

                Data.token = JsonConverter.Deserialize<Token>(response.Content.ReadAsStringAsync().Result);

                timer.Stop();
                ServerConnection.SetToken(Data.token.token);
                MainWindowViewModel.Current.ChangeToApp(loginC.username);
            }
            else
                Error = "Login or password is incorrect";
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Error = "All credentials must be fullfield.";
                return;
            }

            if(Login.Length > 20 || Username.Length > 20 || Password.Length > 20)
            {
                Error = "Max. length for credentials is 20";
                return;
            }

            RegisterCredentials registerC = new RegisterCredentials()
            {
                loginUsername = Login,
                username = Username,
                password = Password
            };

            var response = ServerConnection.Post("/api/Accounts/Register", registerC);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                LoginIntoAccount();
            else
                Error = "Login is taken";
        }
    }
}
