using System.Drawing;
using System.Linq;
using Captura.ViewModels;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RememberByName
    {
        readonly Settings _settings;
        readonly VideoViewModel _videoViewModel;
        readonly AudioSource _audioSource;
        readonly IRegionProvider _regionProvider;
        readonly IWebCamProvider _webCamProvider;
        readonly ScreenShotViewModel _screenShotViewModel;

        readonly ScreenSourceProvider _screenSourceProvider;
        readonly WindowSourceProvider _windowSourceProvider;
        readonly RegionSourceProvider _regionSourceProvider;
        readonly NoVideoSourceProvider _noVideoSourceProvider;
        readonly DeskDuplSourceProvider _deskDuplSourceProvider;

        static readonly RectangleConverter RectangleConverter = new RectangleConverter();

        public RememberByName(Settings Settings,
            VideoViewModel VideoViewModel,
            AudioSource AudioSource,
            IRegionProvider RegionProvider,
            IWebCamProvider WebCamProvider,
            ScreenShotViewModel ScreenShotViewModel,
            // ReSharper disable SuggestBaseTypeForParameter
            ScreenSourceProvider ScreenSourceProvider,
            WindowSourceProvider WindowSourceProvider,
            RegionSourceProvider RegionSourceProvider,
            NoVideoSourceProvider NoVideoSourceProvider,
            DeskDuplSourceProvider DeskDuplSourceProvider
            // ReSharper restore SuggestBaseTypeForParameter
            )
        {
            _settings = Settings;
            _videoViewModel = VideoViewModel;
            _audioSource = AudioSource;
            _regionProvider = RegionProvider;
            _webCamProvider = WebCamProvider;
            _screenShotViewModel = ScreenShotViewModel;
            _screenSourceProvider = ScreenSourceProvider;
            _windowSourceProvider = WindowSourceProvider;
            _regionSourceProvider = RegionSourceProvider;
            _noVideoSourceProvider = NoVideoSourceProvider;
            _deskDuplSourceProvider = DeskDuplSourceProvider;
        }

        public void Remember()
        {
            RememberVideoSource();

            // Remember Video Codec
            _settings.Video.WriterKind = _videoViewModel.SelectedVideoWriterKind.Name;
            _settings.Video.Writer = _videoViewModel.SelectedVideoWriter.ToString();

            // Remember Audio Sources
            _settings.Audio.Microphones = _audioSource.AvailableRecordingSources
                .Where(M => M.Active)
                .Select(M => M.Name)
                .ToArray();

            _settings.Audio.Speakers = _audioSource.AvailableLoopbackSources
                .Where(M => M.Active)
                .Select(M => M.Name)
                .ToArray();

            // Remember ScreenShot Format
            _settings.ScreenShots.ImageFormat = _screenShotViewModel.SelectedScreenShotImageFormat.ToString();

            // Remember ScreenShot Target
            _settings.ScreenShots.SaveTargets = _screenShotViewModel.AvailableImageWriters
                .Where(M => M.Active)
                .Select(M => M.Display)
                .ToArray();

            // Remember Webcam
            _settings.Video.Webcam = _webCamProvider.SelectedCam.Name;
        }

        void RememberVideoSource()
        {
            void SaveSourceName()
            {
                _settings.Video.Source = _videoViewModel.SelectedVideoSourceKind.Source.ToString();
            }

            switch (_videoViewModel.SelectedVideoSourceKind)
            {
                case NoVideoSourceProvider _:
                    _settings.Video.SourceKind = VideoSourceKindEnum.NoVideo;
                    SaveSourceName();
                    break;

                case RegionSourceProvider _:
                    _settings.Video.SourceKind = VideoSourceKindEnum.Region;
                    var rect = _regionProvider.SelectedRegion;
                    _settings.Video.Source = RectangleConverter.ConvertToInvariantString(rect);
                    break;

                case WindowSourceProvider _:
                    _settings.Video.SourceKind = VideoSourceKindEnum.Window;
                    SaveSourceName();
                    break;

                case ScreenSourceProvider _:
                    _settings.Video.SourceKind = VideoSourceKindEnum.Screen;
                    SaveSourceName();
                    break;

                case DeskDuplSourceProvider _:
                    _settings.Video.SourceKind = VideoSourceKindEnum.DeskDupl;
                    SaveSourceName();
                    break;

                default:
                    _settings.Video.SourceKind = VideoSourceKindEnum.FullScreen;
                    _settings.Video.Source = "";
                    break;
            }
        }

        void RestoreVideoSource()
        {
            IScreen GetMatchingScreen()
            {
                return ScreenItem.Enumerate()
                    .Select(M => M.Screen)
                    .FirstOrDefault(M => M.DeviceName == _settings.Video.Source);
            }

            switch (_settings.Video.SourceKind)
            {
                case VideoSourceKindEnum.Region:
                    if (RectangleConverter.ConvertFromInvariantString(_settings.Video.Source) is Rectangle rect)
                    {
                        _regionProvider.SelectedRegion = rect;

                        _videoViewModel.SelectedVideoSourceKind = _regionSourceProvider;
                    }
                    break;

                case VideoSourceKindEnum.NoVideo:
                    var source = _noVideoSourceProvider.Sources.FirstOrDefault(M => M.Name == _settings.Video.Source);

                    if (source != null)
                    {
                        _noVideoSourceProvider.SelectedSource = source;
                        _videoViewModel.SelectedVideoSourceKind = _noVideoSourceProvider;
                    }
                    break;

                case VideoSourceKindEnum.Window:
                    var window = Window.EnumerateVisible().FirstOrDefault(M => M.Title == _settings.Video.Source);

                    if (window != null)
                    {
                        _windowSourceProvider.Set(window.Handle);
                        _videoViewModel.RestoreSourceKind(_windowSourceProvider);
                    }
                    break;

                case VideoSourceKindEnum.Screen:
                {
                    var screen = GetMatchingScreen();

                    if (screen != null)
                    {
                        _screenSourceProvider.Set(screen);
                        _videoViewModel.RestoreSourceKind(_screenSourceProvider);
                    }

                    break;
                }

                case VideoSourceKindEnum.DeskDupl:
                {
                    var screen = GetMatchingScreen();

                    if (screen != null)
                    {
                        _deskDuplSourceProvider.Set(screen);
                        _videoViewModel.RestoreSourceKind(_deskDuplSourceProvider);
                    }

                    break;
                }
            }
        }

        public void RestoreRemembered()
        {
            RestoreVideoSource();

            // Restore Video Codec
            if (!string.IsNullOrEmpty(_settings.Video.WriterKind))
            {
                var kind = _videoViewModel.VideoWriterProviders.FirstOrDefault(W => W.Name == _settings.Video.WriterKind);

                if (kind != null)
                {
                    _videoViewModel.SelectedVideoWriterKind = kind;

                    var codec = _videoViewModel.AvailableVideoWriters.FirstOrDefault(C => C.ToString() == _settings.Video.Writer);

                    if (codec != null)
                        _videoViewModel.SelectedVideoWriter = codec;
                }
            }

            // Restore Microphones
            if (_settings.Audio.Microphones != null)
            {
                foreach (var source in _audioSource.AvailableRecordingSources)
                {
                    source.Active = _settings.Audio.Microphones.Contains(source.Name);
                }
            }

            // Restore Loopback Speakers
            if (_settings.Audio.Speakers != null)
            {
                foreach (var source in _audioSource.AvailableLoopbackSources)
                {
                    source.Active = _settings.Audio.Speakers.Contains(source.Name);
                }
            }

            // Restore ScreenShot Format
            if (!string.IsNullOrEmpty(_settings.ScreenShots.ImageFormat))
            {
                var format = _screenShotViewModel.ScreenShotImageFormats.FirstOrDefault(F => F.ToString() == _settings.ScreenShots.ImageFormat);

                if (format != null)
                    _screenShotViewModel.SelectedScreenShotImageFormat = format;
            }

            // Restore ScreenShot Target
            if (_settings.ScreenShots.SaveTargets != null)
            {
                foreach (var imageWriter in _screenShotViewModel.AvailableImageWriters)
                {
                    imageWriter.Active = _settings.ScreenShots.SaveTargets.Contains(imageWriter.Display);
                }

                // Activate First if none
                if (!_screenShotViewModel.AvailableImageWriters.Any(M => M.Active))
                {
                    _screenShotViewModel.AvailableImageWriters[0].Active = true;
                }
            }
        }
    }
}