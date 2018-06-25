using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace Captura.ImageEditor
{
    public class EllipseStroke : Stroke
    {
        static StylusPointCollection Points(StylusPointCollection StylusPointCollection)
        {
            var start = StylusPointCollection.First().ToPoint();
            var end = StylusPointCollection.Last().ToPoint();

            RectangleDynamicRenderer.Prepare(ref start, ref end, out var w, out var h);

            var center = new Point(start.X + w / 2, start.Y + h / 2);

            var stylusPoints = new StylusPointCollection();

            const double step = 0.05;

            var arc1 = new List<StylusPoint>();
            var arc2 = new List<StylusPoint>();
            var arc3 = new List<StylusPoint>();
            var arc4 = new List<StylusPoint>();

            for (var dx = 0.0; dx < w / 2; dx += step)
            {
                var dy = (h / 2) * Math.Sqrt(1 - Math.Pow(2 * dx / w, 2));

                arc1.Add(new StylusPoint(center.X + dx, center.Y + dy));
                arc2.Add(new StylusPoint(center.X - dx, center.Y + dy));
                arc3.Add(new StylusPoint(center.X - dx, center.Y - dy));
                arc4.Add(new StylusPoint(center.X + dx, center.Y - dy));
            }

            foreach (var point in arc1.AsEnumerable().Reverse())
            {
                stylusPoints.Add(point);
            }

            foreach (var point in arc2)
            {
                stylusPoints.Add(point);
            }

            foreach (var point in arc3.AsEnumerable().Reverse())
            {
                stylusPoints.Add(point);
            }

            foreach (var point in arc4)
            {
                stylusPoints.Add(point);
            }

            stylusPoints.Add(arc1[arc1.Count - 1]);

            return stylusPoints;
        }

        static DrawingAttributes ModifyAttribs(DrawingAttributes DrawingAttributes)
        {
            DrawingAttributes.FitToCurve = true;

            return DrawingAttributes;
        }

        public EllipseStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes)
            : base(Points(StylusPoints), ModifyAttribs(DrawingAttributes)) { }
    }
}