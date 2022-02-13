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
            AvaloniaXamlLoader.Load(this);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            timer.Start();
        }

        double offsss = 0;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            offsss += 5;

            GeneratePath(new Point(0 + offsss *0.3, 300), 500, 150, "Wave3", false);
            GeneratePath(new Point(0 + offsss * 0.6, 450), 600, 100, "Wave2", true);
            GeneratePath(new Point(400 + offsss, 600), 800, 50, "Wave1", false);
        }

        void GeneratePath(Point offset, double width, double height, string objectName, bool startUpside)
        {
            PathGeometry pathGeometry = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(Width + offset.X, offset.Y);
            pathFigure.IsClosed = false;
            pathGeometry.Figures.Add(pathFigure);

            for (int i = 0; i < (Width + offset.X) / width; i++)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point3 = new Point(Width - width * (i + 1) + offset.X, offset.Y);
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width/2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(Width - width * i + offset.X, offset.Y);


                pathFigure.Segments.Add(bezierSegment);
            }

            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, Height);
            pathFigure.Segments.Add(lineSegment);

            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = new Point(Width, Height);
            pathFigure.Segments.Add(lineSegment2);

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
