using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Captura.Properties;
using Screna;
using Screna.Avi;
using System.IO;
using System;

namespace Captura
{
    public class VideoViewModel : ViewModelBase
    {
        public VideoViewModel()
        {
            if (ScreenItem.Count > 1)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Screen, "Screen"));

            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe")))
                AvailableVideoWriterKinds.Add(VideoWriterKind.FFMpeg);

            RefreshCodecs();

            RefreshVideoSources();
        }

        public void RefreshVideoSources()
        {
            AvailableVideoSources.Clear();

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    AvailableVideoSources.Add(WindowItem.Desktop);
                    AvailableVideoSources.Add(WindowItem.TaskBar);

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowItem(win));

                    SelectedVideoSource = WindowItem.Desktop;
                    break;

                case VideoSourceKind.Screen:
                    foreach (var screen in ScreenItem.Enumerate())
                        AvailableVideoSources.Add(screen);

                    SelectedVideoSource = AvailableVideoSources[0];
                    break;

                case VideoSourceKind.Region:
                    AvailableVideoSources.Add(new RegionItem());

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
            AvailableVideoWriters.Clear();

            switch (SelectedVideoWriterKind)
            {
                case VideoWriterKind.SharpAvi:
                    foreach (var codec in AviWriter.EnumerateEncoders())
                    {
                        var item = new SharpAviItem(codec);

                        AvailableVideoWriters.Add(item);

                        if (codec == AviCodec.MotionJpeg)
                            SelectedVideoWriter = item;
                    }
                    break;

                case VideoWriterKind.Gif:
                    AvailableVideoWriters.Add(GifItem.Instance);

                    SelectedVideoWriter = GifItem.Instance;
                    break;

                case VideoWriterKind.FFMpeg:
                    AvailableVideoWriters.Add(FFMpegItem.Instance);

                    SelectedVideoWriter = FFMpegItem.Instance;
                    break;
            }
        }

        public ObservableCollection<VideoWriterKind> AvailableVideoWriterKinds { get; } = new ObservableCollection<VideoWriterKind>
        {
            VideoWriterKind.Gif,
            VideoWriterKind.SharpAvi
        };

        public ObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; } = new ObservableCollection<IVideoWriterItem>();

        VideoWriterKind _writerKind = VideoWriterKind.SharpAvi;

        public VideoWriterKind SelectedVideoWriterKind
        {
            get { return _writerKind; }
            set
            {
                if (_writerKind == value)
                    return;

                _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }
        
        public ObservableCollection<KeyValuePair<VideoSourceKind, string>> AvailableVideoSourceKinds { get; } = new ObservableCollection<KeyValuePair<VideoSourceKind, string>>
        {
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.NoVideo, "No Video"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Window, "Window"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Region, "Region")
        };

        public ObservableCollection<IVideoItem> AvailableVideoSources { get; } = new ObservableCollection<IVideoItem>();

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

        IVideoItem _videoSource = WindowItem.Desktop;

        public IVideoItem SelectedVideoSource
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

        IVideoWriterItem _writer;

        public IVideoWriterItem SelectedVideoWriter
        {
            get { return _writer; }
            set
            {
                _writer = value ?? (AvailableVideoWriters.Count == 0 ? null : AvailableVideoWriters[0]);

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