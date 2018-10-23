using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace Captura.ImageEditor
{
    public class RectangleStroke : Stroke
    {
        static StylusPointCollection Points(StylusPointCollection StylusPointCollection)
        {
            var start = StylusPointCollection.First().ToPoint();
            var end = StylusPointCollection.Last().ToPoint();

            RectangleDynamicRenderer.Prepare(ref start, ref end, out var _, out var _);

            return new StylusPointCollection(new []
            {
                start,
                new Point(start.X, end.Y),
                end,
                new Point(end.X, start.Y),
                start
            });
        }

        static DrawingAttributes ModifyAttribs(DrawingAttributes DrawingAttributes)
        {
            DrawingAttributes.FitToCurve = false;

            return DrawingAttributes;
        }

        public RectangleStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes)
            : base(Points(StylusPoints), ModifyAttribs(DrawingAttributes)) { }
    }
}