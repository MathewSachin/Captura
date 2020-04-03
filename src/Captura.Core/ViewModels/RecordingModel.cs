using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Captura.Audio;
using Captura.FFmpeg;
using Captura.Loc;
using Captura.Models;
using Captura.MouseKeyHook;
using Captura.MouseKeyHook.Steps;
using Captura.Video;
using Captura.Webcam;
using Microsoft.Win32;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingModel : ViewModelBase, IDisposable
    {
        #region Fields
        IRecorder _recorder;
        readonly SyncContextManager _syncContext = new SyncContextManager();

        readonly ISystemTray _systemTray;
        readonly IPreviewWindow _previewWindow;
        readonly WebcamModel _webcamModel;
        readonly TimerModel _timerModel;
        readonly IMessageProvider _messageProvider;
        readonly IFFmpegViewsProvider _ffmpegViewsProvider;

        readonly KeymapViewModel _keymap;
        readonly IAudioSource _audioSource;
        readonly IFpsManager _fpsManager;

        const int StepsRecorderFrameRate = 1;
        #endregion

        public RecordingModel(Settings Settings,
            ILocalizationProvider Loc,
            ISystemTray SystemTray,
            IPreviewWindow PreviewWindow,
            IAudioSource AudioSource,
            WebcamModel WebcamModel,
            KeymapViewModel Keymap,
            TimerModel TimerModel,
            IMessageProvider MessageProvider,
            IFFmpegViewsProvider FFmpegViewsProvider,
            IFpsManager FpsManager) : base(Settings, Loc)
        {
            _systemTray = SystemTray;
            _previewWindow = PreviewWindow;
            _audioSource = AudioSource;
            _webcamModel = WebcamModel;
            _keymap = Keymap;
            _timerModel = TimerModel;
            _messageProvider = MessageProvider;
            _ffmpegViewsProvider = FFmpegViewsProvider;
            _fpsManager = FpsManager;

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            TimerModel.CountdownElapsed += InternalStartRecording;
        }

        RecorderState _recorderState = RecorderState.NotRecording;

        public RecorderState RecorderState
        {
            get => _recorderState;
            private set => Set(ref _recorderState, value);
        }

        void SystemEvents_PowerModeChanged(object Sender, PowerModeChangedEventArgs E)
        {
            if (E.Mode == PowerModes.Suspend && RecorderState == RecorderState.Recording)
            {
                OnPauseExecute();
            }
        }

        public void Dispose()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

        INotification _pauseNotification;

        public void OnPauseExecute()
        {
            // Resume
            if (RecorderState == RecorderState.Paused)
            {
                _systemTray.HideNotification();

                _recorder.Start();
                _timerModel.Resume();

                RecorderState = RecorderState.Recording;

                _pauseNotification?.Remove();
            }
            else // Pause
            {
                _recorder.Stop();
                _timerModel.Pause();

                RecorderState = RecorderState.Paused;

                _pauseNotification = new TextNotification(Loc.Paused, OnPauseExecute);
                _systemTray.ShowNotification(_pauseNotification);
            }
        }

        bool GetImageProviderSafe(Func<IImageProvider> Getter, RecordingModelParams RecordingParams, out IImageProvider ImageProvider)
        {
            ImageProvider = null;

            try
            {
                ImageProvider = Getter();

                return true;
            }
            catch (WindowClosedException e)
            {
                _messageProvider.ShowException(e, "Window Closed");

                return false;
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, Loc.ErrorOccurred);

                return false;
            }
        }

        bool SetupVideoRecorder(IAudioProvider AudioProvider, RecordingModelParams RecordingParams, IMouseKeyHook MouseKeyHook)
        {
            IImageProvider ImgProviderGetter() => GetImageProviderWithOverlays(RecordingParams, MouseKeyHook);

            if (!GetImageProviderSafe(ImgProviderGetter, RecordingParams, out var imgProvider))
                return false;

            IVideoFileWriter videoEncoder;

            try
            {
                videoEncoder = GetVideoFileWriterWithPreview(imgProvider, AudioProvider, RecordingParams);
            }
            catch (FFmpegNotFoundException)
            {
                _ffmpegViewsProvider.ShowUnavailable();

                imgProvider?.Dispose();

                return false;
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                imgProvider?.Dispose();

                return false;
            }

            _recorder = new Recorder(videoEncoder, imgProvider, Settings.Video.FrameRate, AudioProvider, _fpsManager);

            var webcamMode = RecordingParams.VideoSourceKind is WebcamSourceProvider;

            // Separate file for webcam
            if (!webcamMode
                && !(_webcamModel.SelectedCam is NoWebcamItem)
                && Settings.WebcamOverlay.SeparateFile)
            {
                SeparateFileForWebcam(RecordingParams);
            }

            // Separate file for every audio source
            if (Settings.Audio.SeparateFilePerSource)
            {
                SeparateFileForEveryAudioSource(RecordingParams);
            }

            return true;
        }

        bool SetupAudioProvider(RecordingModelParams RecordingParams, out IAudioProvider AudioProvider)
        {
            AudioProvider = null;

            try
            {
                if (!Settings.Audio.SeparateFilePerSource)
                {
                    AudioProvider = _audioSource.GetAudioProvider(
                        Settings.Audio.RecordMicrophone ? RecordingParams.Microphone : null,
                        Settings.Audio.RecordSpeaker ? RecordingParams.Speaker : null);
                }
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                return false;
            }

            return true;
        }

        bool SetupStepsRecorder(RecordingModelParams RecordingParams)
        {
            IImageProvider ImgProviderGetter() => GetImageProvider(RecordingParams);

            if (!GetImageProviderSafe(ImgProviderGetter, RecordingParams, out var imgProvider))
                return false;

            IVideoFileWriter videoEncoder;

            try
            {
                videoEncoder = GetVideoFileWriterWithPreview(imgProvider, null, RecordingParams);
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                imgProvider?.Dispose();

                return false;
            }

            var mouseKeyHook = new MouseKeyHook.MouseKeyHook();

            _recorder = new StepsRecorder(mouseKeyHook,
                videoEncoder,
                imgProvider,
                Settings.Clicks,
                Settings.Keystrokes,
                Settings.Steps,
                _keymap);
            
            return true;
        }

        public bool StartRecording(RecordingModelParams RecordingParams, string FileName = null)
        {
            IsVideo = !(RecordingParams.VideoSourceKind is NoVideoSourceProvider);

            var extension = RecordingParams.VideoWriter.Extension;

            if (RecordingParams.VideoSourceKind?.Source is NoVideoItem x)
                extension = x.Extension;

            CurrentFileName = Settings.GetFileName(extension, FileName);

            if (Settings.Video.RecorderMode == RecorderMode.Steps)
            {
                if (!SetupStepsRecorder(RecordingParams))
                    return false;
            }
            else
            {
                if (!SetupAudioProvider(RecordingParams, out var audioProvider))
                    return false;

                if (IsVideo)
                {
                    var mouseKeyHook = new MouseKeyHook.MouseKeyHook();

                    if (!SetupVideoRecorder(audioProvider, RecordingParams, mouseKeyHook))
                    {
                        mouseKeyHook.Dispose();

                        audioProvider?.Dispose();

                        return false;
                    }
                }
                else if (RecordingParams.VideoSourceKind?.Source is NoVideoItem audioWriter)
                {
                    if (!InitAudioRecorder(audioWriter, audioProvider, RecordingParams))
                    {
                        audioProvider?.Dispose();

                        return false;
                    }
                }
            }


            RecorderState = RecorderState.Recording;

            _recorder.ErrorOccurred += OnErrorOccured;

            if (Settings.PreStartCountdown == 0)
            {
                InternalStartRecording();
            }

            _timerModel.Start();

            return true;
        }

        IRecorder GetAudioRecorder(NoVideoItem AudioWriter, IAudioProvider AudioProvider, string AudioFileName = null)
        {
            var audioFileWriter = AudioWriter.AudioWriterItem.GetAudioFileWriter(
                AudioFileName ?? CurrentFileName,
                AudioProvider?.WaveFormat,
                Settings.Audio.Quality);

            return new AudioRecorder(audioFileWriter, AudioProvider);
        }

        string GetAudioFileName(int Index)
        {
            return Path.ChangeExtension(CurrentFileName,
                $".{Index}{Path.GetExtension(CurrentFileName)}");
        }

        bool InitAudioRecorder(NoVideoItem AudioWriter, IAudioProvider AudioProvider, RecordingModelParams RecordingParams)
        {
            try
            {
                // Not separate files or not multiple sources
                if (!Settings.Audio.SeparateFilePerSource || !(Settings.Audio.RecordMicrophone && Settings.Audio.RecordSpeaker))
                {
                    _recorder = GetAudioRecorder(AudioWriter, AudioProvider);
                }
                else
                {
                    var audioProviders = new[]
                    {
                        _audioSource.GetAudioProvider(RecordingParams.Microphone, null),
                        _audioSource.GetAudioProvider(null, RecordingParams.Speaker)
                    }
                    .Where(M => M != null)
                    .ToArray();

                    var recorders = audioProviders
                        .Select((M, Index) => GetAudioRecorder(AudioWriter, M, GetAudioFileName(Index)))
                        .ToArray();

                    _recorder = new MultiRecorder(recorders);

                    // Set to first file
                    CurrentFileName = GetAudioFileName(0);
                }
            }
            catch (FFmpegNotFoundException)
            {
                _ffmpegViewsProvider.ShowUnavailable();

                return false;
            }

            return true;
        }

        void SeparateFileForWebcam(RecordingModelParams RecordingParams)
        {
            var webcamImgProvider = new WebcamImageProvider(_webcamModel);

            var webcamFileName = Path.ChangeExtension(CurrentFileName, $".webcam{Path.GetExtension(CurrentFileName)}");

            var webcamVideoWriter = GetVideoFileWriter(webcamImgProvider, null, RecordingParams, webcamFileName);

            var webcamRecorder = new Recorder(webcamVideoWriter, webcamImgProvider, Settings.Video.FrameRate);

            _recorder = new MultiRecorder(_recorder, webcamRecorder);
        }

        void SeparateFileForEveryAudioSource(RecordingModelParams RecordingParams)
        {
            var audioWriter = new WaveItem();

            IRecorder GetAudioRecorder(IAudioProvider AudioProvider, string AudioFileName = null)
            {
                return new AudioRecorder(
                    audioWriter.GetAudioFileWriter(AudioFileName ?? CurrentFileName, AudioProvider?.WaveFormat,
                        Settings.Audio.Quality), AudioProvider);
            }

            string GetAudioFileName(int Index)
            {
                return Path.ChangeExtension(CurrentFileName, $".{Index}.wav");
            }

            var audioProviders = new[]
            {
                _audioSource.GetAudioProvider(RecordingParams.Microphone, null),
                _audioSource.GetAudioProvider(null, RecordingParams.Speaker)
            }
            .Where(M => M != null)
            .ToArray();

            if (audioProviders.Length > 0)
            {
                var recorders = audioProviders
                    .Select((M, Index) => GetAudioRecorder(M, GetAudioFileName(Index)))
                    .Concat(new[] {_recorder})
                    .ToArray();

                _recorder = new MultiRecorder(recorders);
            }
        }

        void InternalStartRecording()
        {
            _recorder.Start();
        }

        void OnErrorOccured(Exception E)
        {
            _syncContext.Run(() =>
            {
                var cancelled = E is WindowClosedException;

                AfterRecording();

                if (!cancelled)
                    _messageProvider.ShowException(E, E.Message);
            });
        }

        void AfterRecording()
        {
            _pauseNotification?.Remove();

            RecorderState = RecorderState.NotRecording;

            _recorder.ErrorOccurred -= OnErrorOccured;
            _recorder = null;

            _timerModel.Stop();
        }

        IVideoFileWriter GetVideoFileWriterWithPreview(IImageProvider ImgProvider, IAudioProvider AudioProvider, RecordingModelParams RecordingParams)
        {
            return RecordingParams.VideoSourceKind is NoVideoSourceProvider
                ? null
                : new WithPreviewWriter(GetVideoFileWriter(ImgProvider, AudioProvider, RecordingParams), _previewWindow);
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider, RecordingModelParams RecordingParams, string FileName = null)
        {
            if (RecordingParams.VideoSourceKind is NoVideoSourceProvider)
                return null;

            var args = new VideoWriterArgs
            {
                FileName = FileName ?? CurrentFileName,
                FrameRate = Settings.Video.FrameRate,
                VideoQuality = Settings.Video.Quality,
                ImageProvider = ImgProvider,
                AudioQuality = Settings.Audio.Quality,
                AudioProvider = AudioProvider
            };

            if (Settings.Video.RecorderMode == RecorderMode.Replay)
            {
                return new FFmpegReplayWriter(args,
                    Settings.Video.ReplayDuration,
                    M => RecordingParams.VideoWriter.GetVideoFileWriter(M));
            }
            else return RecordingParams.VideoWriter.GetVideoFileWriter(args);
        }

        IEnumerable<IOverlay> GetOverlays(RecordingModelParams RecordingParams, IMouseKeyHook MouseKeyHook)
        {
            // No mouse and webcam overlays in webcam mode
            var webcamMode = RecordingParams.VideoSourceKind is WebcamSourceProvider;

            yield return new CensorOverlay(Settings.Censored);

            if (!webcamMode && !Settings.WebcamOverlay.SeparateFile)
            {
                yield return new WebcamOverlay(_webcamModel, Settings);
            }

            if (!webcamMode)
            {
                yield return new MousePointerOverlay(Settings.MousePointerOverlay);
            }

            var clickSettings = webcamMode
                ? new MouseClickSettings { Display = false }
                : Settings.Clicks;

            yield return new MouseKeyOverlay(MouseKeyHook,
                clickSettings,
                Settings.Keystrokes,
                _keymap,
                CurrentFileName,
                () => _timerModel.TimeSpan);

            yield return new ElapsedOverlay(Settings.Elapsed, () => _timerModel.TimeSpan);

            // Text Overlays
            foreach (var overlay in Settings.TextOverlays)
            {
                yield return new CustomOverlay(overlay);
            }

            // Image Overlays
            foreach (var overlay in Settings.ImageOverlays.Where(M => M.Display))
            {
                IOverlay imgOverlay = null;

                try
                {
                    imgOverlay = new CustomImageOverlay(overlay);
                }
                catch
                {
                    // Ignore Errors like Image not found, Invalid Image
                }

                if (imgOverlay != null)
                    yield return imgOverlay;
            }
        }

        IImageProvider GetImageProviderWithOverlays(RecordingModelParams RecordingParams, IMouseKeyHook MouseKeyHook)
        {
            var imageProvider = GetImageProvider(RecordingParams);

            return imageProvider == null
                ? null
                : new OverlayedImageProvider(imageProvider, GetOverlays(RecordingParams, MouseKeyHook).ToArray());
        }

        IImageProvider GetImageProvider(RecordingModelParams RecordingParams)
        {
            return RecordingParams
                .VideoSourceKind
                ?.Source
                ?.GetImageProvider(Settings.IncludeCursor);
        }

        public string CurrentFileName { get; private set; }
        public bool IsVideo { get; private set; }

        public async Task StopRecording()
        {
            // Reference Recorder as it will be set to null
            var rec = _recorder;

            var task = Task.Run(() => rec.Dispose());

            AfterRecording();

            await task;
        }
    }
}