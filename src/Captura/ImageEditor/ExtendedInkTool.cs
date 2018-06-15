using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;

namespace Captura
{
    public class ExtendedInkTool
    {
        public ExtendedInkTool(string ToolName,
            InkCanvasEditingMode EditingMode,
            Func<DynamicRenderer> DynamicRendererFunc = null,
            Cursor Cursor = null)
        {
            this.ToolName = ToolName;
            this.EditingMode = EditingMode;
            this.Cursor = Cursor;
            this.DynamicRendererFunc = DynamicRendererFunc;
        }

        public InkCanvasEditingMode EditingMode { get; }

        public string ToolName { get; }

        public Func<DynamicRenderer> DynamicRendererFunc { get; }

        public Cursor Cursor { get; }

        public const string Pen = nameof(Pen);
        public const string Eraser = nameof(Eraser);
        public const string StrokeEraser = "Stroke Eraser";
        public const string Line = nameof(Line);

        public static ExtendedInkTool[] Tools { get; } =
        {
            new ExtendedInkTool(Pen, InkCanvasEditingMode.Ink, () => new DynamicRenderer(), Cursors.Pen),
            new ExtendedInkTool(Eraser, InkCanvasEditingMode.EraseByPoint),
            new ExtendedInkTool(StrokeEraser, InkCanvasEditingMode.EraseByStroke),
            new ExtendedInkTool(Line, InkCanvasEditingMode.Ink, () => new LineDynamicRenderer())
        };
    }
}