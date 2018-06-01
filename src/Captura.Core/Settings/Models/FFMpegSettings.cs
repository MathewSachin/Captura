using System.Collections.ObjectModel;

namespace Captura
{
    public class FFMpegSettings : PropertyStore
    {
        public string FolderPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public string TwitchKey
        {
            get => Get("");
            set => Set(value);
        }

        public string YouTubeLiveKey
        {
            get => Get("");
            set => Set(value);
        }

        public ObservableCollection<CustomFFMpegCodec> CustomCodecs { get; } = new ObservableCollection<CustomFFMpegCodec>();

        public string CustomStreamingUrl
        {
            get => Get("rtmp://");
            set => Set(value);
        }

        public bool Resize
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int ResizeWidth
        {
            get => Get(640);
            set => Set(value);
        }

        public int ResizeHeight
        {
            get => Get(480);
            set => Set(value);
        }

        public X264Settings X264 { get; } = new X264Settings();
    }
}