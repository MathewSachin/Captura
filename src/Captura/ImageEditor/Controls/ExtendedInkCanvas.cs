using System.Windows.Controls;
using System.Windows.Ink;

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
            void AddCustomStroke(Stroke CustomStroke)
            {
                Strokes.Remove(E.Stroke);

                // Remove two history items
                if (DataContext is ImageEditorViewModel vm)
                {
                    vm.RemoveLastHistory();
                    vm.RemoveLastHistory();
                }

                Strokes.Add(CustomStroke);

                var args = new InkCanvasStrokeCollectedEventArgs(CustomStroke);

                base.OnStrokeCollected(args);
            }

            if (DynamicRenderer is IDynamicRenderer renderer)
            {
                AddCustomStroke(renderer.GetStroke(E.Stroke.StylusPoints, E.Stroke.DrawingAttributes));
            }
            else base.OnStrokeCollected(E);
        }
    }
}