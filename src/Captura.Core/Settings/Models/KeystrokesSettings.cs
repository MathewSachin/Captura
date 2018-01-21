namespace Captura
{
    public class KeystrokesSettings : TextOverlaySettings
    {
        public int MaxTextLength
        {
            get => Get(15);
            set => Set(value);
        }

        public int Timeout
        {
            get => Get(2);
            set => Set(value);
        }

        public int HistoryCount
        {
            get => Get(6);
            set => Set(value);
        }

        public int HistorySpacing
        {
            get => Get(10);
            set => Set(value);
        }
    }
}