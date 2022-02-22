using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Views
{
    public partial class LoginPanel
    {
        double currentOffset = 0;

        //Klatka animacji wszystkie fale
        void Timer_Tick(object? sender, EventArgs e)
        {
            currentOffset += 5;

            //Offsety są już ustawione
            GeneratePath(new Point(0 + currentOffset * 0.3, Height - 420), 500, 150, "Wave3", false);
            GeneratePath(new Point(0 + currentOffset * 0.6, Height - 250), 600, 100, "Wave2", true);
            GeneratePath(new Point(400 + currentOffset, Height - 100), 800, 50, "Wave1", false);
        }

        /// <summary>
        /// Generacja klatki animacji jednej ścieżki (fali)
        /// </summary>
        /// <param name="offset">Offset od którego ma być generowana mapa (liczy się od prawego rogu)</param>
        /// <param name="width">szerokość fali</param>
        /// <param name="height">wysokość fali</param>
        /// <param name="objectName">nazwa obiektu do którego ma zostać przypisana fala (musi byc ustawiona atrybutem Name="")</param>
        /// <param name="startUpside">czy ma zostać odwrócona, zaczynać od dołu, a nie od wzgórza</param>
        void GeneratePath(Point offset, double width, double height, string objectName, bool startUpside)
        {
            PathGeometry pathGeometry = new PathGeometry();

            //W związku z tym że render dla ułatwienia animacji rysowana jest od prawego rogu okna,
            //Do starting point dodaje całą szerokośc okna
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(Width + offset.X, offset.Y);
            pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);

            //Segmenty fali, rysowanie na całej płaszczyźnie ekranu
            for (int i = 0; i < (Width + offset.X) / width; i++)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point3 = new Point(Width - width * (i + 1) + offset.X, offset.Y);
                bezierSegment.Point2 = new Point(bezierSegment.Point3.X + width / 2, offset.Y + height * (i % 2 == Convert.ToInt32(startUpside) ? -1 : 1));
                bezierSegment.Point1 = new Point(Width - width * i + offset.X, offset.Y);

                pathFigure.Segments.Add(bezierSegment);
            }

            //Łączenie ostatniego segmentu z lewym dolnym rogiem okna
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = new Point(0, Height);
            pathFigure.Segments.Add(lineSegment);

            //Następnie przeciągniecie linii do prawego dolnego
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
