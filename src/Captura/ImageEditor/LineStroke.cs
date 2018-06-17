using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura
{
    public class LineStroke : Stroke
    {
        public LineStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes) : base(StylusPoints, DrawingAttributes) { }

        protected override void DrawCore(DrawingContext DrawingContext, DrawingAttributes DrawingAttribs)
        {
            LineDynamicRenderer.Draw(DrawingContext, 
                StylusPoints.First().ToPoint(),
                StylusPoints.Last().ToPoint(),
                new Pen(new SolidColorBrush(DrawingAttribs.Color), DrawingAttribs.Width));
        }
    }
}