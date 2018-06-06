using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Ink;

namespace Captura
{
    public class StrokeHistory : HistoryItem
    {
        public int EditingOperationCount { get; set; }

        public InkCanvasEditingMode EditingMode { get; set; }

        public List<Stroke> Added { get; } = new List<Stroke>();
        public List<Stroke> Removed { get; } = new List<Stroke>();
    }
}