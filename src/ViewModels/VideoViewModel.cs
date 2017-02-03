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

                case VideoSourceKind.Region:
                    AvailableVideoSources.Add(new RegionVSLI());

                    SelectedVideoSource = AvailableVideoSources[0];
                    break;
            }

            if (SelectedVideoSourceKind == VideoSourceKind.Region)
                RegionSelector.Instance.Show();
            else RegionSelector.Instance.Hide();
        }

        public void RefreshCodecs()
        {
            // Available Codecs
            AvailableCodecs.Clear();
            AvailableCodecs.Add(new AviCodec("Gif"));
            
            foreach (var codec in AviWriter.EnumerateEncoders())
                AvailableCodecs.Add(codec);

            SelectedCodec = AviCodec.MotionJpeg;
        }

        public ObservableCollection<AviCodec> AvailableCodecs { get; } = new ObservableCollection<AviCodec>();

        public ObservableCollection<KeyValuePair<VideoSourceKind, string>> AvailableVideoSourceKinds { get; } = new ObservableCollection<KeyValuePair<VideoSourceKind, string>>
        {
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.NoVideo, "No Video"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Window, "Window"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Region, "Region")
        };

        public ObservableCollection<IVSLI> AvailableVideoSources { get; } = new ObservableCollection<IVSLI>();

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

        IVSLI _videoSource = WindowVSLI.Desktop;

        public IVSLI SelectedVideoSource
        {
            get { return _videoSource; }
            set
            {
                if (value == null && AvailableVideoSources.Count > 0)
                    value = AvailableVideoSources[0];

                _videoSource = value;

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