using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura.ImageEditor
{
    public class EllipseStroke : Stroke
    {
        public EllipseStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes) : base(StylusPoints, DrawingAttributes) { }

        protected override void DrawCore(DrawingContext DrawingContext, DrawingAttributes DrawingAttribs)
        {
            EllipseDynamicRenderer.Draw(DrawingContext,
                StylusPoints.First().ToPoint(),
                StylusPoints.Last().ToPoint(),
                new Pen(new SolidColorBrush(DrawingAttribs.Color), DrawingAttribs.Width));
        }
    }
}