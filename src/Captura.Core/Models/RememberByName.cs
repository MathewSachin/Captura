﻿using System.Collections.Generic;
using System.Linq;
using Captura.ViewModels;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RememberByName
    {
        readonly Settings _settings;
        readonly VideoSourcesViewModel _videoSourcesViewModel;
        readonly VideoWritersViewModel _videoWritersViewModel;
        readonly AudioSource _audioSource;
        readonly IWebCamProvider _webCamProvider;
        readonly ScreenShotViewModel _screenShotViewModel;
        readonly IEnumerable<IVideoSourceProvider> _videoSourceProviders;

        public RememberByName(Settings Settings,
            VideoSourcesViewModel VideoSourcesViewModel,
            VideoWritersViewModel VideoWritersViewModel,
            AudioSource AudioSource,
            IWebCamProvider WebCamProvider,
            ScreenShotViewModel ScreenShotViewModel,
            IEnumerable<IVideoSourceProvider> VideoSourceProviders)
        {
            _settings = Settings;
            _videoSourcesViewModel = VideoSourcesViewModel;
            _videoWritersViewModel = VideoWritersViewModel;
            _audioSource = AudioSource;
            _webCamProvider = WebCamProvider;
            _screenShotViewModel = ScreenShotViewModel;
            _videoSourceProviders = VideoSourceProviders;
        }

        public void Remember()
        {
            // Remember Video Source
            _settings.Video.SourceKind = _videoSourcesViewModel.SelectedVideoSourceKind.Name;
            _settings.Video.Source = _videoSourcesViewModel.SelectedVideoSourceKind.Serialize();

            // Remember Video Codec
            _settings.Video.WriterKind = _videoWritersViewModel.SelectedVideoWriterKind.Name;
            _settings.Video.Writer = _videoWritersViewModel.SelectedVideoWriter.ToString();

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

        void RestoreVideoSource()
        {
            if (string.IsNullOrEmpty(_settings.Video.SourceKind))
                return;

            var provider = _videoSourceProviders.FirstOrDefault(M => M.Name == _settings.Video.SourceKind);

            if (provider == null)
                return;

            if (provider.Deserialize(_settings.Video.Source))
            {
                _videoSourcesViewModel.RestoreSourceKind(provider);
            }
        }

        void RestoreVideoCodec()
        {
            if (string.IsNullOrEmpty(_settings.Video.WriterKind))
                return;

            var kind = _videoWritersViewModel.VideoWriterProviders.FirstOrDefault(W => W.Name == _settings.Video.WriterKind);

            if (kind == null)
                return;

            _videoWritersViewModel.SelectedVideoWriterKind = kind;

            var codec = _videoWritersViewModel.AvailableVideoWriters.FirstOrDefault(C => C.ToString() == _settings.Video.Writer);

            if (codec != null)
                _videoWritersViewModel.SelectedVideoWriter = codec;
        }

        public void RestoreRemembered()
        {
            RestoreVideoSource();

            RestoreVideoCodec();

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