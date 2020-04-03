using Captura.Video;

namespace Captura.MouseKeyHook
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

        public bool ShowRepeatCounter
        {
            get => Get(true);
            set => Set(value);
        }

        public string KeymapName
        {
            get => Get(KeymapViewModel.DefaultKeymapFileName);
            set => Set(value);
        }

        public bool SeparateTextFile
        {
            get => Get(false);
            set => Set(value);
        }
    }
}