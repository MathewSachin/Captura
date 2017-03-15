using Captura.Properties;

namespace Captura
{
    public class SettingsViewModel : ViewModelBase
    {
        readonly Settings _provider = Settings.Default;
        
        public int VideoQuality
        {
            get { return _provider.VideoQuality; }
            set
            {
                if (VideoQuality == value)
                    return;

                _provider.VideoQuality = value;

                OnPropertyChanged();
            }
        }

        public int AudioQuality
        {
            get { return _provider.AudioQuality; }
            set
            {
                if (AudioQuality == value)
                    return;

                _provider.AudioQuality = value;

                OnPropertyChanged();
            }
        }

        public int FrameRate
        {
            get { return _provider.FrameRate; }
            set
            {
                if (FrameRate == value)
                    return;

                _provider.FrameRate = value;

                OnPropertyChanged();
            }
        }
        
        public bool GifUnconstrained
        {
            get { return _provider.GifUnconstrained; }
            set
            {
                if (GifUnconstrained == value)
                    return;

                _provider.GifUnconstrained = value;

                OnPropertyChanged();
            }
        }

        public string OutPath
        {
            get { return _provider.OutputPath; }
            set
            {
                if (OutPath == value)
                    return;

                _provider.OutputPath = value;

                OnPropertyChanged();
            }
        }

        public bool MinimizeOnStart
        {
            get { return _provider.MinimizeOnStart; }
            set
            {
                if (MinimizeOnStart == value)
                    return;

                _provider.MinimizeOnStart = value;

                OnPropertyChanged();
            }
        }

        public bool MinimizeToTray
        {
            get { return _provider.MinTray; }
            set
            {
                if (_provider.MinTray == value)
                    return;

                _provider.MinTray = value;

                OnPropertyChanged();
            }
        }

        public bool IncludeCursor
        {
            get { return _provider.IncludeCursor; }
            set
            {
                if (IncludeCursor == value)
                    return;

                _provider.IncludeCursor = value;

                OnPropertyChanged();
            }
        }

        public bool MouseClicks
        {
            get { return Settings.Default.MouseClicks; }
            set
            {
                if (MouseClicks == value)
                    return;

                Settings.Default.MouseClicks = value;

                OnPropertyChanged();
            }
        }

        public bool KeyStrokes
        {
            get { return Settings.Default.KeyStrokes; }
            set
            {
                if (KeyStrokes == value)
                    return;

                Settings.Default.KeyStrokes = value;

                OnPropertyChanged();
            }
        }

        public void Save() => _provider.Save();
    }
}
