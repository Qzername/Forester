using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Forester.ViewModels;
using Forester.Views;
using System;
using System.Diagnostics;

namespace Forester.Controls
{
    public partial class Wave : UserControl
    {
        public static readonly AvaloniaProperty<int> WaveWidthProperty = AvaloniaProperty.RegisterAttached<Wave, int>(nameof(WaveWidth), typeof(Wave));
        public static readonly AvaloniaProperty<int> WaveHeightProperty = AvaloniaProperty.RegisterAttached<Wave, int>(nameof(WaveHeight), typeof(Wave));
        public static readonly AvaloniaProperty<bool> IsInvertedProperty = AvaloniaProperty.RegisterAttached<Wave, bool>(nameof(IsInverted), typeof(Wave));
        public static readonly AvaloniaProperty<int> WidthOffsetProperty = AvaloniaProperty.RegisterAttached<Wave, int>(nameof(WidthOffset), typeof(Wave));
        public static readonly AvaloniaProperty<int> HeightOffsetProperty = AvaloniaProperty.RegisterAttached<Wave, int>(nameof(HeightOffset), typeof(Wave));
        public static readonly AvaloniaProperty<int> FPSProperty = AvaloniaProperty.RegisterAttached<Wave, int>(nameof(FPS), typeof(Wave));
        public static readonly AvaloniaProperty<Color> FillColorProperty = AvaloniaProperty.RegisterAttached<Wave, Color>(nameof(FillColor), typeof(Wave));

        /* 
         * note:
         * This code is quite old and it's mostly copied from Forester 2.0v
         * im not bothering editing it yet
         */

        //Properties:
        public int WaveWidth { get; set; }
        public int WaveHeight { get; set; }
        public bool IsInverted { get; set; }
        public int WidthOffset { get; set; }
        public int HeightOffset { get; set; }
        public int FPS { get; set; }
        public Color FillColor { get; set; }

        DispatcherTimer timer;
        double currentOffset;

        public Wave()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            WavePath.Fill = new SolidColorBrush(FillColor);

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / FPS); //co 15 ms nowa klatka co daje nam 1000/15= 66,(6) FPS
            timer.Start();
        }

        //Klatka animacji wszystkie fale
        void Timer_Tick(object? sender, EventArgs e)
        {
            currentOffset += 5;

            var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

            Width = desktop.MainWindow!.Width;
            Height = desktop.MainWindow!.Height;

            //Offsety s¹ ju¿ ustawione
            WavePath.Data = GeneratePath(new Point(WidthOffset + currentOffset *0.3, Height - HeightOffset), WaveWidth, WaveHeight, IsInverted);
        }

        /// <summary>
        /// Generacja klatki animacji jednej œcie¿ki (fali)
        /// </summary>
        /// <param name="offset">Offset od którego ma byæ generowana mapa (liczy siê od prawego rogu)</param>
        /// <param name="width">szerokoœæ fali</param>
        /// <param name="height">wysokoœæ fali</param>
        /// <param name="startUpside">czy ma zostaæ odwrócona, zaczynaæ od do³u, a nie od wzgórza</param>
        Geometry GeneratePath(Point offset, double width, double height, bool startUpside)
        {
            PathGeometry pathGeometry = new PathGeometry();

            //W zwi¹zku z tym ¿e render dla u³atwienia animacji rysowana jest od prawego rogu okna,
            //Do starting point dodaje ca³¹ szerokoœc okna
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(Width + offset.X, offset.Y);
            pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);

            //Segmenty fali, rysowanie na ca³ej p³aszczyŸnie ekranu
            for (int i = 0; i < (Width + offset.X) / width; i++)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point3 = new Point(Width - width * (i + 1) + offset.X, offset.Y);
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width / 2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(Width - width * i + offset.X, offset.Y);

                pathFigure.Segments.Add(bezierSegment);
            }

            //£¹czenie ostatniego segmentu z lewym dolnym rogiem okna
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, Height);
            pathFigure.Segments.Add(lineSegment);

            //Nastêpnie przeci¹gniecie linii do prawego dolnego
            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = new Point(Width + offset.X, Height);
            pathFigure.Segments.Add(lineSegment2);

            //i na koniec do punktu startowego
            LineSegment lineSegment3 = new LineSegment();
            lineSegment3.Point = new Point(Width + offset.X, offset.Y);
            pathFigure.Segments.Add(lineSegment3);

            return pathGeometry;
        }
    }
}
