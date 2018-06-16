using System.Linq;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using Captura.ImageEditor;

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

            switch (DynamicRenderer)
            {
                case LineDynamicRenderer _:
                    AddCustomStroke(new Stroke(new StylusPointCollection(new []
                    {
                        E.Stroke.StylusPoints.First(),
                        E.Stroke.StylusPoints.Last()
                    }), E.Stroke.DrawingAttributes));
                    break;

                case RectangleDynamicRenderer _:
                    AddCustomStroke(new RectangleStroke(E.Stroke.StylusPoints, E.Stroke.DrawingAttributes));
                    break;

                default:
                    base.OnStrokeCollected(E);
                    break;
            }
        }
    }
}