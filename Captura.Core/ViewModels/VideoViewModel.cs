using Captura.Models;
using Screna;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        public VideoViewModel()
        {
            // Check if there are multiple Screens
            if (ScreenItem.Count > 1)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Screen, "Screen"));

            // Check if SharpAvi is available
            if (File.Exists("SharpAvi.dll"))
            {
                AvailableVideoWriterKinds.Add(VideoWriterKind.SharpAvi);

                SelectedVideoWriterKind = VideoWriterKind.SharpAvi;
            }

            // Check if FFMpeg is available
            if (File.Exists("ffmpeg.exe"))
            {
                AvailableVideoWriterKinds.Add(VideoWriterKind.FFMpeg);

                SelectedVideoWriterKind = VideoWriterKind.FFMpeg;
            }
           
            RefreshCodecs();

            RefreshVideoSources();
        }

        public void RefreshVideoSources()
        {
            AvailableVideoSources.Clear();

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    // Desktop would be default
                    AvailableVideoSources.Add(WindowItem.Desktop);
                    AvailableVideoSources.Add(WindowItem.TaskBar);

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowItem(win));
                    break;

                case VideoSourceKind.Screen:
                    foreach (var screen in ScreenItem.Enumerate())
                        AvailableVideoSources.Add(screen);
                    break;

                case VideoSourceKind.Region:
                    AvailableVideoSources.Add(ServiceProvider.Get<IVideoItem>(ServiceName.RegionSource));
                    break;
            }

            // Set first source as default
            if (AvailableVideoSources.Count > 0)
                SelectedVideoSource = AvailableVideoSources[0];

            // RegionSelector should only be shown on Region Capture.
            ServiceProvider.Get<Action<bool>>(ServiceName.RegionSelectorVisibility).Invoke(SelectedVideoSourceKind == VideoSourceKind.Region);
        }
        
        void InitSharpAviCodecs()
        {
            foreach (var codec in AviWriter.EnumerateEncoders())
            {
                var item = new SharpAviItem(codec);

                AvailableVideoWriters.Add(item);

                // Set MotionJpeg as default
                if (codec == AviCodec.MotionJpeg)
                    SelectedVideoWriter = item;
            }
        }

        public void RefreshCodecs()
        {
            // Available Codecs
            AvailableVideoWriters.Clear();

            switch (SelectedVideoWriterKind)
            {
                case VideoWriterKind.SharpAvi:
                    InitSharpAviCodecs();
                    break;

                case VideoWriterKind.Gif:
                    AvailableVideoWriters.Add(GifItem.Instance);

                    SelectedVideoWriter = GifItem.Instance;
                    break;

                case VideoWriterKind.FFMpeg:
                    foreach (var item in FFMpegItem.Items)
                        AvailableVideoWriters.Add(item);

                    SelectedVideoWriter = AvailableVideoWriters[0];
                    break;
            }
        }

        public ObservableCollection<VideoWriterKind> AvailableVideoWriterKinds { get; } = new ObservableCollection<VideoWriterKind>
        {
            // Gif is always availble
            VideoWriterKind.Gif
        };

        public ObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; } = new ObservableCollection<IVideoWriterItem>();

        // Give SharpAvi the default preference
        VideoWriterKind _writerKind = VideoWriterKind.Gif;

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
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Region, "Region"),
            new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.WebCam, "WebCam")
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
    }
}