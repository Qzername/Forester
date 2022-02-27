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
        SolidColorBrush _first;
        SolidColorBrush _second;
        SolidColorBrush _third;

        public SolidColorBrush first
        {
            get => _first;
            set => this.RaiseAndSetIfChanged(ref _first, value);
        }

        public SolidColorBrush second
        {
            get => _second;
            set => this.RaiseAndSetIfChanged(ref _second, value);
        }

        public SolidColorBrush third
        {
            get => _third;
            set => this.RaiseAndSetIfChanged(ref _third, value);
        }

        public string _login;
        public string _username;
        public string _password;
        public string _error;
        public bool _rememberMe;

        public string login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        public string username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }

        public bool rememberMe
        {
            get => _rememberMe;
            set => this.RaiseAndSetIfChanged(ref _rememberMe, value);
        }

        bool isLogin;

        bool _loginIsEnabled, _registerIsEnabled;
        float _loginOpacity, _registerOpacity;

        public bool loginIsEnabled
        {
            get => _loginIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _loginIsEnabled, value);
        }

        public bool registerIsEnabled
        {
            get => _registerIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _registerIsEnabled, value);
        }

        public float loginOpacity
        {
            get => _loginOpacity;
            set => this.RaiseAndSetIfChanged(ref _loginOpacity, value);
        }

        public float registerOpacity
        {
            get => _registerOpacity;
            set => this.RaiseAndSetIfChanged(ref _registerOpacity, value);
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

        MainWindowViewModel mainWindowVM;

        public LoginPanelViewModel(MainWindowViewModel mainWindowVM)
        {
            this.mainWindowVM = mainWindowVM;

            SetTheme();

            isLogin = true;
            loginIsEnabled = true;
            loginOpacity = 1f;

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
            GeneratePath(new Point(0 + currentOffset * 0.3, mainWindowVM.height - 420), 500, 150, ref tempWave1, false);
            GeneratePath(new Point(0 + currentOffset * 0.6, mainWindowVM.height - 250), 600, 100, ref tempWave2, true);
            GeneratePath(new Point(400 + currentOffset, mainWindowVM.height - 100), 800, 50, ref tempWave3, false);

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
            pathFigure.StartPoint = new Point(mainWindowVM.width + offset.X, offset.Y);
            pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);

            //Segmenty fali, rysowanie na całej płaszczyźnie ekranu
            for (int i = 0; i < (mainWindowVM.width + offset.X) / width; i++)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point3 = new Point(mainWindowVM.width - width * (i + 1) + offset.X, offset.Y);
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width / 2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(mainWindowVM.width - width * i + offset.X, offset.Y);

                pathFigure.Segments.Add(bezierSegment);
            }

            //Łączenie ostatniego segmentu z lewym dolnym rogiem okna
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, mainWindowVM.height);
            pathFigure.Segments.Add(lineSegment);

            //Następnie przeciągniecie linii do prawego dolnego
            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = new Point(mainWindowVM.width, mainWindowVM.height);
            pathFigure.Segments.Add(lineSegment2);

            //i na koniec do punktu startowego
            LineSegment lineSegment3 = new LineSegment();
            lineSegment3.Point = new Point(mainWindowVM.width + offset.X, offset.Y);
            pathFigure.Segments.Add(lineSegment3);

            Path path = new Path();
            path.Stretch = Stretch.None;
            path.Data = pathGeometry;

            geometry = path.Data;
        }

        public void SwitchPanels(Button button)
        {
            isLogin = !isLogin;

            loginIsEnabled = isLogin;
            loginOpacity = isLogin ? 1 : 0;

            registerIsEnabled = !isLogin;
            registerOpacity = isLogin ? 0 : 1;

            button.Content = isLogin ? "Don't have an account?" : "Already have an account?";

            login = string.Empty;
            username = string.Empty;
            password = string.Empty;
            error = string.Empty;
        }

        public void Exit(Window window) => window.Close();

        public void Login()
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                error = "Login and password cannot be empty.";
                return;
            }

            LoginCredentials loginC = new LoginCredentials()
            {
                username = login,
                password = password
            };

            var response = ServerConnection.Post("/api/Accounts/Login", loginC);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //zalogowano
                //remember me
                if (rememberMe)
                {
                    var config = Data.config;
                    config.autoLogin = login;
                    config.autoPassword = password;
                    Data.SaveConfig(config);
                }

                Data.token = JsonConverter.Deserialize<Token>(response.Content.ReadAsStringAsync().Result);

                timer.Stop();
                mainWindowVM.ChangeToApp(loginC.username);
            }
            else
                error = "Login or password is incorrect";
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                error = "All credentials must be fullfield.";
                return;
            }

            if(login.Length > 20 || username.Length > 20 || password.Length > 20)
            {
                error = "Max. length for credentials is 20";
                return;
            }

            RegisterCredentials registerC = new RegisterCredentials()
            {
                loginUsername = login,
                username = username,
                password = password
            };

            var response = ServerConnection.Post("/api/Accounts/Register", registerC);

            System.Diagnostics.Debug.WriteLine(response.StatusCode + " " + response.Content.ReadAsStringAsync().Result);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                Login();
            else
                error = "Login is taken";
        }

        /// <summary>
        /// Ustawienie kolorów
        /// </summary>
        void SetTheme()
        {
            first = new SolidColorBrush(HexToColor(Data.config.theme.colorFirst));
            second = new SolidColorBrush(HexToColor(Data.config.theme.colorSecond));
            third = new SolidColorBrush(HexToColor(Data.config.theme.colorThird));
        }

        /// <summary>
        /// Zamiana hex na kolor (hex może ale nie musi zawierać #)
        /// </summary>
        Color HexToColor(string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }
    }
}
