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
        readonly AudioSourceViewModel _audioSourceViewModel;
        readonly WebcamModel _webcamModel;
        readonly ScreenShotModel _screenShotModel;
        readonly IEnumerable<IVideoSourceProvider> _videoSourceProviders;

        public RememberByName(Settings Settings,
            VideoSourcesViewModel VideoSourcesViewModel,
            VideoWritersViewModel VideoWritersViewModel,
            AudioSourceViewModel AudioSourceViewModel,
            ScreenShotModel ScreenShotModel,
            IEnumerable<IVideoSourceProvider> VideoSourceProviders,
            WebcamModel WebcamModel)
        {
            _settings = Settings;
            _videoSourcesViewModel = VideoSourcesViewModel;
            _videoWritersViewModel = VideoWritersViewModel;
            _audioSourceViewModel = AudioSourceViewModel;
            _screenShotModel = ScreenShotModel;
            _videoSourceProviders = VideoSourceProviders;
            _webcamModel = WebcamModel;
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
            _settings.Audio.Microphones = _audioSourceViewModel.AvailableRecordingSources
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => !M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            _settings.Audio.Speakers = _audioSourceViewModel.AvailableRecordingSources
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            // Remember ScreenShot Target
            _settings.ScreenShots.SaveTargets = _screenShotModel.AvailableImageWriters
                .Where(M => M.Active)
                .Select(M => M.Display)
                .ToArray();

            // Remember Webcam
            _settings.Video.Webcam = _webcamModel.SelectedCam.Name;
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
                foreach (var source in _audioSourceViewModel.AvailableRecordingSources.Where(M => !M.Item.IsLoopback))
                {
                    source.IsActive = _settings.Audio.Microphones.Contains(source.Item.Name);
                }
            }

            // Restore Loopback Speakers
            if (_settings.Audio.Speakers != null)
            {
                foreach (var source in _audioSourceViewModel.AvailableRecordingSources.Where(M => M.Item.IsLoopback))
                {
                    source.IsActive = _settings.Audio.Speakers.Contains(source.Item.Name);
                }
            }

            // Restore ScreenShot Target
            if (_settings.ScreenShots.SaveTargets != null)
            {
                foreach (var imageWriter in _screenShotModel.AvailableImageWriters)
                {
                    imageWriter.Active = _settings.ScreenShots.SaveTargets.Contains(imageWriter.Display);
                }

                // Activate First if none
                if (!_screenShotModel.AvailableImageWriters.Any(M => M.Active))
                {
                    _screenShotModel.AvailableImageWriters[0].Active = true;
                }
            }

            // Restore Webcam
            if (!string.IsNullOrEmpty(_settings.Video.Webcam))
            {
                var webcam = _webcamModel.AvailableCams.FirstOrDefault(C => C.Name == _settings.Video.Webcam);

                if (webcam != null)
                {
                    _webcamModel.SelectedCam = webcam;
                }
            }
        }
    }
}