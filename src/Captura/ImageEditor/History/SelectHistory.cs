using System.Windows;
using System.Windows.Ink;

namespace Captura
{
    public class SelectHistory : HistoryItem
    {
        public int EditingOperationCount { get; set; }

        public StrokeCollection Selection { get; set; }

        public Rect OldRect { get; set; }

        public Rect NewRect { get; set; }
    }
}