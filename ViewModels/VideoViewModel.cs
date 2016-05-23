using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
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

                    foreach (var win in WindowHandler.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowVSLI(win.Handle));
                    break;

                case VideoSourceKind.Screen:
                    foreach (var Screen in ScreenVSLI.Enumerate())
                        AvailableVideoSources.Add(Screen);
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
                if (_videoSource == value)
                    return;

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
                if (_codec == value)
                    return;

                _codec = value;

                OnPropertyChanged();
            }
        }

        int _quality = 70;

        public int Quality
        {
            get { return _quality; }
            set
            {
                if (_quality == value)
                    return;

                _quality = value;
                
                OnPropertyChanged();
            }
        }

        int _frameRate = 10;

        public int FrameRate
        {
            get { return _frameRate; }
            set
            {
                if (_frameRate == value)
                    return;

                _frameRate = value;

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