using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Captura.ViewModels;

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

        static readonly RectangleConverter RectangleConverter = new RectangleConverter();

        public RememberByName(Settings Settings,
            VideoViewModel VideoViewModel,
            AudioSource AudioSource,
            IRegionProvider RegionProvider,
            IWebCamProvider WebCamProvider,
            ScreenShotViewModel ScreenShotViewModel)
        {
            _settings = Settings;
            _videoViewModel = VideoViewModel;
            _audioSource = AudioSource;
            _regionProvider = RegionProvider;
            _webCamProvider = WebCamProvider;
            _screenShotViewModel = ScreenShotViewModel;
        }

        public void Remember()
        {
            // Remember Video Source
            _settings.Video.SourceKind = _videoViewModel.SelectedVideoSourceKind.Name;

            switch (_videoViewModel.SelectedVideoSourceKind)
            {
                case RegionSourceProvider _:
                    var rect = _regionProvider.SelectedRegion;
                    _settings.Video.Source = RectangleConverter.ConvertToInvariantString(rect);
                    break;

                default:
                    _settings.Video.Source = _videoViewModel.SelectedVideoSource.ToString();
                    break;
            }

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
            _settings.ScreenShots.SaveTargets = _videoViewModel.AvailableImageWriters
                .Where(M => M.Active)
                .Select(M => M.Display)
                .ToArray();

            // Remember Webcam
            _settings.Video.Webcam = _webCamProvider.SelectedCam.Name;
        }

        public void RestoreRemembered()
        {
            // Restore Video Source
            if (!string.IsNullOrEmpty(_settings.Video.SourceKind))
            {
                var kind = _videoViewModel.AvailableVideoSourceKinds.FirstOrDefault(M => M.Name == _settings.Video.SourceKind);

                if (kind != null)
                {
                    _videoViewModel.SelectedVideoSourceKind = kind;

                    switch (kind)
                    {
                        case RegionSourceProvider _:
                            if (RectangleConverter.ConvertFromInvariantString(_settings.Video.Source) is Rectangle rect)
                               _regionProvider.SelectedRegion = rect;
                            break;

                        default:
                            var source = _videoViewModel.AvailableVideoSources
                                .FirstOrDefault(S => S.ToString() == _settings.Video.Source);

                            if (source != null)
                                _videoViewModel.SelectedVideoSource = source;
                            break;
                    }
                }
            }

            // Restore Video Codec
            if (!string.IsNullOrEmpty(_settings.Video.WriterKind))
            {
                var kind = _videoViewModel.AvailableVideoWriterKinds.FirstOrDefault(W => W.Name == _settings.Video.WriterKind);

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
                foreach (var imageWriter in _videoViewModel.AvailableImageWriters)
                {
                    imageWriter.Active = _settings.ScreenShots.SaveTargets.Contains(imageWriter.Display);
                }

                // Activate First if none
                if (!_videoViewModel.AvailableImageWriters.Any(M => M.Active))
                {
                    _videoViewModel.AvailableImageWriters[0].Active = true;
                }
            }
        }
    }
}