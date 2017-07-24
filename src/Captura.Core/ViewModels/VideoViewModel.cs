using Captura.Models;
using Captura.Models.VideoItems;
using Captura.Properties;
using Screna;
using System.Collections.ObjectModel;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        public IRegionProvider RegionProvider { get; private set; }

        private bool availableRegionSizeVisibility = false;

        public bool AvailableRegionSizeVisibility
        {
            get { return availableRegionSizeVisibility; }
            set
            {
                if (availableRegionSizeVisibility != value)
                {
                    availableRegionSizeVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void Init()
        {
            RegionProvider = ServiceProvider.Get<IRegionProvider>(ServiceName.RegionProvider);
            
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
                    SelectedVideoSourceKind = VideoSourceKind.Screen;
            };

            this.RegionProvider.RegionSizeChanged += () => {
                var rect = this.RegionProvider.SelectedRegion;
                if (!(this.LastSelectedRegionSize.Width == rect.Width && this.LastSelectedRegionSize.Height == rect.Height))
                {
                    this.IsCustomResized = true;
                    this.SelectedRegionSizeKind = RegionSize.Custom;
                    this.LastSelectedRegionSize = rect;
                }
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

            this.AvailableRegionSizeVisibility = false;

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    AvailableVideoSources.Add(WindowItem.TaskBar);

                    // Prevent RegionSelector from showing here
                    RegionProvider.SelectorVisible = false;

                    foreach (var win in Window.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowItem(win));
                    
                    break;

                case VideoSourceKind.Screen:
                    AvailableVideoSources.Add(FullScreenItem.Instance);

                    foreach (var screen in ScreenItem.Enumerate())
                        AvailableVideoSources.Add(screen);
                    break;

                case VideoSourceKind.Region:
                    this.AvailableRegionSizeVisibility = true;
                    AvailableVideoSources.Add(RegionProvider.VideoSource);
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
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Screen, nameof(Resources.Screen)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Window, nameof(Resources.Window)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Region, nameof(Resources.Region))
        };

        public ObservableCollection<IVideoItem> AvailableVideoSources { get; } = new ObservableCollection<IVideoItem>();

        VideoSourceKind _videoSourceKind = VideoSourceKind.Region;

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

        IVideoItem _videoSource = FullScreenItem.Instance;

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

        public ObservableCollection<RegionSize> AvailableRegionSizeKinds { get; } = new ObservableCollection<RegionSize>() {
            RegionSize.XVGA_640_480,
            RegionSize.XVGA_800_600,
            RegionSize.WVGA_854_480,
            RegionSize.YOUTUBE_940_530,
            RegionSize.XGA_1024_768,
            RegionSize.HD720_1280_720,
            RegionSize.SXGA_1280_1024,
            RegionSize.HD1080_1920_1080,
            RegionSize.WUXGA_1920_1200,
            RegionSize.Custom
        };

        private RegionSize selectedRegionSizeKind = RegionSize.YOUTUBE_940_530;

        public RegionSize SelectedRegionSizeKind
        {
            get { return selectedRegionSizeKind; }
            set
            {
                selectedRegionSizeKind = value;

                OnPropertyChanged();

                if (!this.IsCustomResized)
                {
                    RefreshRegionSize();
                }
                else
                {
                    this.IsCustomResized = false;
                }
            }
        }

        /// <summary>
        /// 마지막 선택된 영역 화면 사이즈 체크
        /// </summary>
        private Rectangle LastSelectedRegionSize { get; set; } = Rectangle.Empty;
        private bool IsCustomResized { get; set; } = false;

        private void RefreshRegionSize()
        {
            var width = 940;
            var height = 530;

            var SelectedRegionSizeKindString = this.SelectedRegionSizeKind.ToString().Split('_');
            if (SelectedRegionSizeKindString != null && SelectedRegionSizeKindString.Length == 3)
            {
                width = int.Parse(SelectedRegionSizeKindString[1]);
                height = int.Parse(SelectedRegionSizeKindString[2]);
            }

            var regionSize = new Rectangle(this.RegionProvider.SelectedRegion.X, this.RegionProvider.SelectedRegion.Y, width, height + 30);
            this.LastSelectedRegionSize = regionSize;
            this.RegionProvider.SelectedRegion = regionSize;
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
            new ObjectLocalizer<IImageWriterItem>(ClipboardWriter.Instance, nameof(Resources.Clipboard)),
            new ObjectLocalizer<IImageWriterItem>(ImgurWriter.Instance, nameof(Resources.Imgur))
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