using Captura.Models;
using Captura.Properties;
using Screna;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        public IRegionProvider RegionProvider { get; private set; }
        
        public void Init()
        {
            RegionProvider = ServiceProvider.Get<IRegionProvider>(ServiceName.RegionProvider);

            // Check if there are multiple Screens
            if (ScreenItem.Count > 1)
                AvailableVideoSourceKinds.Add(new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Screen, nameof(Resources.Screen)));

            // Check if SharpAvi is available
            if (ServiceProvider.FileExists("SharpAvi.dll"))
            {
                AvailableVideoWriterKinds.Add(VideoWriterKind.SharpAvi);

                SelectedVideoWriterKind = VideoWriterKind.SharpAvi;
            }
            
            // Check if FFMpeg is available
            RefreshFFMpeg();
                       
            RefreshCodecs();

            RefreshVideoSources();

            ServiceProvider.FFMpegPathChanged += RefreshFFMpeg;

            RegionProvider.SelectorHidden += () =>
            {
                if (SelectedVideoSourceKind == VideoSourceKind.Region)
                    SelectedVideoSourceKind = VideoSourceKind.Window;
            };
        }
        
        public void RefreshFFMpeg()
        {
            if (ServiceProvider.FFMpegExists)
            {
                if (!AvailableVideoWriterKinds.Contains(VideoWriterKind.FFMpeg))
                    AvailableVideoWriterKinds.Add(VideoWriterKind.FFMpeg);                    
            }
            else
            {
                if (SelectedVideoWriterKind == VideoWriterKind.FFMpeg)
                    SelectedVideoWriterKind = VideoWriterKind.Gif;

                if (AvailableVideoWriterKinds.Contains(VideoWriterKind.FFMpeg))
                    AvailableVideoWriterKinds.Remove(VideoWriterKind.FFMpeg);
            }
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

                    // Prevent RegionSelector from showing here
                    RegionProvider.SelectorVisible = false;

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowItem(win));
                    
                    break;

                case VideoSourceKind.Screen:
                    foreach (var screen in ScreenItem.Enumerate())
                        AvailableVideoSources.Add(screen);
                    break;

                case VideoSourceKind.Region:
                    AvailableVideoSources.Add(RegionProvider.VideoSource);
                    break;
            }

            // Set first source as default
            if (AvailableVideoSources.Count > 0)
                SelectedVideoSource = AvailableVideoSources[0];

            // RegionSelector should only be shown on Region Capture.
            RegionProvider.SelectorVisible = SelectedVideoSourceKind == VideoSourceKind.Region;
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

                case VideoWriterKind.Folder:
                    var folderItem = new FolderItem();

                    AvailableVideoWriters.Add(folderItem);
                    SelectedVideoWriter = folderItem;

                    break;
            }
        }

        public ObservableCollection<VideoWriterKind> AvailableVideoWriterKinds { get; } = new ObservableCollection<VideoWriterKind>
        {
            // Gif is always availble
            VideoWriterKind.Gif,

            VideoWriterKind.Folder
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
        
        public ObservableCollection<ObjectLocalizer<VideoSourceKind>> AvailableVideoSourceKinds { get; } = new ObservableCollection<ObjectLocalizer<VideoSourceKind>>
        {
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.NoVideo, nameof(Resources.NoVideo)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Window, nameof(Resources.Window)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Region, nameof(Resources.Region))
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

        public ObservableCollection<ObjectLocalizer<IImageWriterItem>> AvailableImageWriters { get; } = new ObservableCollection<ObjectLocalizer<IImageWriterItem>>()
        {
            new ObjectLocalizer<IImageWriterItem>(DiskWriter.Instance, nameof(Resources.Disk)),
            new ObjectLocalizer<IImageWriterItem>(ClipboardWriter.Instance, nameof(Resources.Clipboard))
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