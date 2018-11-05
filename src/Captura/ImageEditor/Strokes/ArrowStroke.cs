using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;

namespace Captura
{
    public class ArrowStroke : Stroke
    {
        static StylusPointCollection Points(StylusPointCollection StylusPoints)
        {
            var start = StylusPoints.First().ToPoint();
            var end = StylusPoints.Last().ToPoint();

            LineDynamicRenderer.Prepare(ref start, ref end);

            ArrowDynamicRenderer.GetArrowPoints(start, end, out var p1, out var p2);

            return new StylusPointCollection(new[] { start, end, p1, end, p2 });
        }

        static DrawingAttributes ModifyAttribs(DrawingAttributes DrawingAttributes)
        {
            DrawingAttributes.FitToCurve = false;

            return DrawingAttributes;
        }

        public ArrowStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes)
            : base(Points(StylusPoints), ModifyAttribs(DrawingAttributes)) { }
    }
}