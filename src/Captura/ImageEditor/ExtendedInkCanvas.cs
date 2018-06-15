using System.Linq;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace Captura
{
    public class ExtendedInkCanvas : InkCanvas
    {
        public void SetInkTool(ExtendedInkTool InkTool)
        {
            EditingMode = InkTool.EditingMode;

            var dynamicRenderer = InkTool.DynamicRendererFunc?.Invoke();

            if (dynamicRenderer != null)
                DynamicRenderer = dynamicRenderer;

            if (InkTool.Cursor != null)
            {
                Cursor = InkTool.Cursor;
                UseCustomCursor = true;
            }
            else UseCustomCursor = false;
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs E)
        {
            if (DynamicRenderer is LineDynamicRenderer)
            {
                Strokes.Remove(E.Stroke);

                // Remove two history items
                if (DataContext is ImageEditorViewModel vm)
                {
                    vm.RemoveLastHistory();
                    vm.RemoveLastHistory();
                }

                var customStroke = new Stroke(new StylusPointCollection(new []
                {
                    E.Stroke.StylusPoints.First(),
                    E.Stroke.StylusPoints.Last()
                }), E.Stroke.DrawingAttributes);

                Strokes.Add(customStroke);

                var args = new InkCanvasStrokeCollectedEventArgs(customStroke);

                base.OnStrokeCollected(args);
            }
            else base.OnStrokeCollected(E);
        }
    }
}