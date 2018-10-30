using System.Windows;

namespace Captura
{
    public class CropHistory : IHistoryItem
    {
        public Rect OldRect { get; set; }

        public Rect NewRect { get; set; }
    }
}
