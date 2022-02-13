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
        double currentOffset = 0;

        public LoginPanel()
        {
            AvaloniaXamlLoader.Load(this);

            //Ustawienie zegaru do animacji
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 15); //co 15 ms nowa klatka co daje nam 1000/15= 66,(6) FPS
            timer.Start();
        }


        //Klatka animacji wszystkie fale
        void Timer_Tick(object? sender, EventArgs e)
        {
            currentOffset += 5;

            //Offsety s¹ ju¿ ustawione
            GeneratePath(new Point(0 + currentOffset * 0.3, 300), 500, 150, "Wave3", false);
            GeneratePath(new Point(0 + currentOffset * 0.6, 450), 600, 100, "Wave2", true);
            GeneratePath(new Point(400 + currentOffset, 600), 800, 50, "Wave1", false);
        }

        /// <summary>
        /// Generacja klatki animacji jednej œcie¿ki (fali)
        /// </summary>
        /// <param name="offset">Offset od którego ma byæ generowana mapa (liczy siê od prawego rogu)</param>
        /// <param name="width">szerokoœæ fali</param>
        /// <param name="height">wysokoœæ fali</param>
        /// <param name="objectName">nazwa obiektu do którego ma zostaæ przypisana fala (musi byc ustawiona atrybutem Name="")</param>
        /// <param name="startUpside">czy ma zostaæ odwrócona, zaczynaæ od do³u, a nie od wzgórza</param>
        void GeneratePath(Point offset, double width, double height, string objectName, bool startUpside)
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
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width/2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(Width - width * i + offset.X, offset.Y);

                pathFigure.Segments.Add(bezierSegment);
            }

            //£¹czenie ostatniego segmentu z lewym dolnym rogiem okna
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, Height);
            pathFigure.Segments.Add(lineSegment);

            //Nastêpnie przeci¹gniecie linii do prawego dolnego
            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = new Point(Width, Height);
            pathFigure.Segments.Add(lineSegment2);

            //i na koniec do punktu startowego
            LineSegment lineSegment3 = new LineSegment();
            lineSegment3.Point = new Point(Width + offset.X, offset.Y);
            pathFigure.Segments.Add(lineSegment3);

            Path path = new Path();
            path.Stretch = Stretch.None;
            path.Data = pathGeometry;

            var pathOBJ = this.FindControl<Path>(objectName);
            pathOBJ.Data = path.Data;
        }
    }
}
