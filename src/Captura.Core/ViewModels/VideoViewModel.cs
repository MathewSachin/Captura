using Captura.Models;
using Screna;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        public void Init()
        {
            // Check if SharpAvi is available
            if (ServiceProvider.FileExists("SharpAvi.dll"))
            {
                AvailableVideoWriterKinds.Add(VideoWriterKind.SharpAvi);
            }
                                               
            RefreshCodecs();

            RefreshVideoSources();
            
            ServiceProvider.RegionProvider.SelectorHidden += () =>
            {
                if (SelectedVideoSourceKind == VideoSourceKind.Region)
                    SelectedVideoSourceKind = VideoSourceKind.Screen;
            };
        }
        
        public void RefreshVideoSources()
        {
            AvailableVideoSources.Clear();

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    AvailableVideoSources.Add(WindowItem.TaskBar);

                    // Prevent RegionSelector from showing here
                    ServiceProvider.RegionProvider.SelectorVisible = false;

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowItem(win));
                    
                    break;

                case VideoSourceKind.Screen:
                    AvailableVideoSources.Add(FullScreenItem.Instance);

                    foreach (var screen in ScreenItem.Enumerate())
                        AvailableVideoSources.Add(screen);
                    break;

                case VideoSourceKind.Region:
                    AvailableVideoSources.Add(ServiceProvider.RegionProvider.VideoSource);
                    break;

                case VideoSourceKind.NoVideo:
                    AvailableVideoSources.Add(WaveItem.Instance);

                    foreach (var item in FFMpegAudioItem.Items)
                        AvailableVideoSources.Add(item);

                    break;
            }

            // Set first source as default
            if (AvailableVideoSources.Count > 0)
                SelectedVideoSource = AvailableVideoSources[0];

            // RegionSelector should only be shown on Region Capture.
            ServiceProvider.RegionProvider.SelectorVisible = SelectedVideoSourceKind == VideoSourceKind.Region;
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
            VideoWriterKind.Gif,

            VideoWriterKind.FFMpeg
        };

        public ObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; } = new ObservableCollection<IVideoWriterItem>();
        
        VideoWriterKind _writerKind = VideoWriterKind.FFMpeg;

        public VideoWriterKind SelectedVideoWriterKind
        {
            get => _writerKind;
            set
            {
                if (_writerKind == value)
                    return;

                _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }
        
        public ObservableCollection<ObjectLocalizer<VideoSourceKind>> AvailableVideoSourceKinds { get; } = new ObservableCollection<ObjectLocalizer<VideoSourceKind>>
        {
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.NoVideo, nameof(LanguageManager.OnlyAudio)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Screen, nameof(LanguageManager.Screen)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Window, nameof(LanguageManager.Window)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Region, nameof(LanguageManager.Region))
        };

        public ObservableCollection<IVideoItem> AvailableVideoSources { get; } = new ObservableCollection<IVideoItem>();

        VideoSourceKind _videoSourceKind = VideoSourceKind.Screen;

        public VideoSourceKind SelectedVideoSourceKind
        {
            get => _videoSourceKind;
            set
            {
                if (_videoSourceKind == value)
                    return;

                _videoSourceKind = value;
                
                OnPropertyChanged();

                RefreshVideoSources();
            }
        }

        IVideoItem _videoSource = FullScreenItem.Instance;

        public IVideoItem SelectedVideoSource
        {
            get => _videoSource;
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
            get => _writer;
            set
            {
                _writer = value ?? (AvailableVideoWriters.Count == 0 ? null : AvailableVideoWriters[0]);

                OnPropertyChanged();
            }
        }

        public ObservableCollection<ObjectLocalizer<IImageWriterItem>> AvailableImageWriters { get; } = new ObservableCollection<ObjectLocalizer<IImageWriterItem>>()
        {
            new ObjectLocalizer<IImageWriterItem>(DiskWriter.Instance, nameof(LanguageManager.Disk)),
            new ObjectLocalizer<IImageWriterItem>(ClipboardWriter.Instance, nameof(LanguageManager.Clipboard)),
            new ObjectLocalizer<IImageWriterItem>(ImgurWriter.Instance, nameof(LanguageManager.Imgur))
        };

        IImageWriterItem _imgWriter = DiskWriter.Instance;

        public IImageWriterItem SelectedImageWriter
        {
            get => _imgWriter;
            set
            {
                _imgWriter = value;

                OnPropertyChanged();
            }
        }
    }
}