namespace Captura
{
    public class KeystrokesSettings : TextOverlaySettings
    {
        public int MaxTextLength { get; set; } = 15;

        public int Timeout { get; set; } = 2;

        public int HistoryCount { get; set; } = 6;

        public int HistorySpacing { get; set; } = 10;
    }
}