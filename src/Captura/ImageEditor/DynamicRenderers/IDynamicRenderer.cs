using System.Windows.Ink;
using System.Windows.Input;

namespace Captura
{
    public interface IDynamicRenderer
    {
        Stroke GetStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes);
    }
}