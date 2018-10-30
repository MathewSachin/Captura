namespace Captura
{
    public class HistoryState : IHistoryItem
    {
        public ImageEffect Effect { get; set; }

        public int Brightness { get; set; }

        public int Contrast { get; set; }

        public int Rotation { get; set; }

        public bool FlipX { get; set; }

        public bool FlipY { get; set; }
    }
}