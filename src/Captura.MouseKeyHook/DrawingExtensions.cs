using System;
using System.Drawing;

namespace Captura.Models
{
    static class DrawingExtensions
    {
        public static void DrawArrow(this IEditableFrame Frame, Point Start, Point End, Color Color)
        {
            const int LineWidth = 5;

            Frame.DrawLine(Start, End, Color, LineWidth);

            GetArrowPoints(Start, End, out var p1, out var p2);

            Frame.DrawLine(End, p1, Color, LineWidth);
            Frame.DrawLine(End, p2, Color, LineWidth);
        }

        const double ArrowAngle = Math.PI / 180 * 150;
        const int ArrowLength = 15;

        static void GetArrowPoints(Point Start, Point End, out Point P1, out Point P2)
        {
            var theta = Math.Atan2(End.Y - Start.Y, End.X - Start.X);

            P1 = new Point((int)(End.X + ArrowLength * Math.Cos(theta + ArrowAngle)),
                (int)(End.Y + ArrowLength * Math.Sin(theta + ArrowAngle)));

            P2 = new Point((int)(End.X + ArrowLength * Math.Cos(theta - ArrowAngle)),
                (int)(End.Y + ArrowLength * Math.Sin(theta - ArrowAngle)));
        }
    }
}
