using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;

namespace Captura
{
    public class LineStroke : Stroke
    {
        static StylusPointCollection Points(StylusPointCollection StylusPoints)
        {
            var start = StylusPoints.First().ToPoint();
            var end = StylusPoints.Last().ToPoint();

            LineDynamicRenderer.Prepare(ref start, ref end);

            return new StylusPointCollection(new[] { start, end });
        }

        public LineStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes) : base(Points(StylusPoints), DrawingAttributes) { }
    }
}