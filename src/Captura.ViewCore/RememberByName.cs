using System.Collections.Generic;
using System.Linq;
using Captura.Video;
using Captura.ViewModels;
using Captura.Webcam;

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
            _settings.Video.SourceKind = _videoSourcesViewModel
                .SelectedVideoSourceKind
                .Name;

            _settings.Video.Source = _videoSourcesViewModel
                .SelectedVideoSourceKind
                .Serialize();

            // Remember Video Codec
            _settings.Video.WriterKind = _videoWritersViewModel
                .SelectedVideoWriterKind
                .Name;

            _settings.Video.Writer = _videoWritersViewModel
                .SelectedVideoWriter
                .ToString();

            // Remember Post Writer
            _settings.Video.PostWriter = _videoWritersViewModel.SelectedPostWriter.ToString();

            // Remember Audio Sources
            _settings.Audio.Microphone = _audioSourceViewModel.SelectedMicrophone?.Name;

            _settings.Audio.Speaker = _audioSourceViewModel.SelectedSpeaker?.Name;

            // Remember ScreenShot Target
            _settings.ScreenShots.SaveTargets = _screenShotModel
                .AvailableImageWriters
                .Where(M => M.Active)
                .Select(M => M.Display)
                .ToArray();

            // Remember Webcam
            _settings.Video.Webcam = _webcamModel
                .SelectedCam
                .Name;

            // Remember Steps writer
            _settings.Steps.Writer = _videoWritersViewModel
                .SelectedStepsWriter
                ?.ToString();
        }

        void RestoreVideoSource()
        {
            if (string.IsNullOrEmpty(_settings.Video.SourceKind))
                return;

            var provider = _videoSourceProviders
                .FirstOrDefault(M => M.Name == _settings.Video.SourceKind);

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

            var kind = _videoWritersViewModel
                .VideoWriterProviders
                .FirstOrDefault(W => W.Name == _settings.Video.WriterKind);

            if (kind == null)
                return;

            _videoWritersViewModel.SelectedVideoWriterKind = kind;

            var codec = _videoWritersViewModel
                .AvailableVideoWriters
                .FirstOrDefault(C => C.ToString() == _settings.Video.Writer);

            if (codec != null)
                _videoWritersViewModel.SelectedVideoWriter = codec;
        }

        public void RestoreRemembered()
        {
            RestoreVideoSource();

            RestoreVideoCodec();

            // Restore Post Writer
            var codec = _videoWritersViewModel.AvailablePostWriters.FirstOrDefault(C => C.ToString() == _settings.Video.PostWriter);

            if (codec != null)
                _videoWritersViewModel.SelectedPostWriter = codec;

            // Restore Microphone
            var mic = _audioSourceViewModel.AvailableMicrophones.FirstOrDefault(M => M.Name == _settings.Audio.Microphone);

            if (mic != null)
                _audioSourceViewModel.SelectedMicrophone = mic;

            // Restore Loopback Speaker
            var speaker = _audioSourceViewModel.AvailableSpeakers.FirstOrDefault(M => M.Name == _settings.Audio.Speaker);

            if (speaker != null)
                _audioSourceViewModel.SelectedSpeaker = speaker;

            // Restore ScreenShot Target
            if (_settings.ScreenShots.SaveTargets != null)
            {
                foreach (var imageWriter in _screenShotModel.AvailableImageWriters)
                {
                    imageWriter.Active = _settings
                        .ScreenShots
                        .SaveTargets
                        .Contains(imageWriter.Display);
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
                var webcam = _webcamModel
                    .AvailableCams
                    .FirstOrDefault(C => C.Name == _settings.Video.Webcam);

                if (webcam != null)
                {
                    _webcamModel.SelectedCam = webcam;
                }
            }

            // Restore Steps writer
            if (!string.IsNullOrEmpty(_settings.Steps.Writer))
            {
                var stepsWriter = _videoWritersViewModel
                    .AvailableStepWriters
                    .FirstOrDefault(M => M.ToString() == _settings.Steps.Writer);

                if (stepsWriter != null)
                {
                    _videoWritersViewModel.SelectedStepsWriter = stepsWriter;
                }
            }
        }
    }
}