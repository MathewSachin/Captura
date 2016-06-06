using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Captura.Properties;
using Screna;
using Screna.Avi;

namespace Captura
{
    public class VideoViewModel : ViewModelBase
    {
        public VideoViewModel()
        {
            if (ScreenVSLI.Count > 1)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Screen, "Screen"));

            RefreshCodecs();

            RefreshVideoSources();
        }

        public void RefreshVideoSources()
        {
            AvailableVideoSources.Clear();

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    AvailableVideoSources.Add(WindowVSLI.Desktop);
                    AvailableVideoSources.Add(WindowVSLI.TaskBar);

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowVSLI(win));

                    SelectedVideoSource = WindowVSLI.Desktop;
                    break;

                case VideoSourceKind.Screen:
                    foreach (var screen in ScreenVSLI.Enumerate())
                        AvailableVideoSources.Add(screen);

                    SelectedVideoSource = AvailableVideoSources[0];
                    break;
            }
        }

        public void RefreshCodecs()
        {
            // Available Codecs
            AvailableCodecs.Clear();
            AvailableCodecs.Add(new AviCodec("Gif"));

            foreach (var Codec in AviWriter.EnumerateEncoders())
                AvailableCodecs.Add(Codec);

            SelectedCodec = AviCodec.MotionJpeg;
        }

        public ObservableCollection<AviCodec> AvailableCodecs { get; } = new ObservableCollection<AviCodec>();

        public ObservableCollection<KeyValuePair<VideoSourceKind, string>> AvailableVideoSourceKinds { get; } = new ObservableCollection<KeyValuePair<VideoSourceKind, string>>
        {
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.NoVideo, "No Video"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Window, "Window")
        };

        public ObservableCollection<IVideoSourceListItem> AvailableVideoSources { get; } = new ObservableCollection<IVideoSourceListItem>();

        VideoSourceKind _videoSourceKind = VideoSourceKind.Window;

        public VideoSourceKind SelectedVideoSourceKind
        {
            get { return _videoSourceKind; }
            set
            {
                if (_videoSourceKind == value)
                    return;

                _videoSourceKind = value;
                
                OnPropertyChanged();

                RefreshVideoSources();
            }
        }

        IVideoSourceListItem _videoSource = WindowVSLI.Desktop;

        public IVideoSourceListItem SelectedVideoSource
        {
            get { return _videoSource; }
            set
            {
                _videoSource = value ?? WindowVSLI.Desktop;

                OnPropertyChanged();
            }
        }

        AviCodec _codec = AviCodec.MotionJpeg;

        public AviCodec SelectedCodec
        {
            get { return _codec; }
            set
            {
                _codec = value ?? AviCodec.MotionJpeg;

                OnPropertyChanged();
            }
        }
        
        public int Quality
        {
            get { return Settings.Default.VideoQuality; }
            set
            {
                if (Quality == value)
                    return;

                Settings.Default.VideoQuality = value;
                
                OnPropertyChanged();
            }
        }
        
        public int FrameRate
        {
            get { return Settings.Default.FrameRate; }
            set
            {
                if (FrameRate == value)
                    return;

                Settings.Default.FrameRate = value;

                OnPropertyChanged();
            }
        }

        Color _bgColor = Colors.Transparent;

        public Color BackgroundColor
        {
            get { return _bgColor; }
            set
            {
                if (_bgColor == value)
                    return;

                _bgColor = value;

                OnPropertyChanged();
            }
        }
    }
}