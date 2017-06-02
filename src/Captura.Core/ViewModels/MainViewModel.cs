using Captura.Models;
using Captura.Properties;
using Screna;
using Screna.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using Window = Screna.Window;

namespace Captura.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        Timer _timer;
        IRecorder _recorder;
        string _currentFileName;
        MouseCursor _cursor;
        bool isVideo;
        public static readonly RectangleConverter RectangleConverter = new RectangleConverter();
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        IWebCamProvider _webCamProvider;

        public bool WebCamVisible
        {
            get => _webCamProvider.IsVisible;
            set
            {
                _webCamProvider.IsVisible = value;

                OnPropertyChanged();
            }
        }
        #endregion
        
        public MainViewModel()
        {
            #region Commands
            ScreenShotCommand = new DelegateCommand(CaptureScreenShot);
            
            ScreenShotActiveCommand = new DelegateCommand(() => SaveScreenShot(ScreenShotWindow(Window.ForegroundWindow)));

            ScreenShotDesktopCommand = new DelegateCommand(() => SaveScreenShot(ScreenShotWindow(Window.DesktopWindow)));

            RecordCommand = new DelegateCommand(() =>
            {
                if (RecorderState == RecorderState.NotRecording)
                    StartRecording();
                else StopRecording();
            });

            RefreshCommand = new DelegateCommand(() =>
            {
                VideoViewModel.RefreshVideoSources();

                VideoViewModel.RefreshCodecs();

                AudioViewModel.AudioSource.Refresh();

                Status.LocalizationKey = nameof(Resources.Refreshed);
            });
            
            PauseCommand = new DelegateCommand(() =>
            {
                if (RecorderState == RecorderState.Paused)
                {
                    ServiceProvider.SystemTray.HideNotification();

                    _recorder.Start();
                    _timer?.Start();

                    RecorderState = RecorderState.Recording;
                    Status.LocalizationKey = nameof(Resources.Recording);
                }
                else
                {
                    _recorder.Stop();
                    _timer?.Stop();

                    RecorderState = RecorderState.Paused;
                    Status.LocalizationKey = nameof(Resources.Paused);

                    ServiceProvider.SystemTray.ShowTextNotification(Resources.Paused, 3000, null);
                }
            }, false);
            #endregion
        }

        void RestoreRemembered()
        {
            #region Restore Video Source
            if (Settings.LastSourceKind == VideoSourceKind.Window)
            {
                VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Window;

                var source = VideoViewModel.AvailableVideoSources.FirstOrDefault(window => window.ToString() == Settings.LastSourceName);

                if (source != null)
                    VideoViewModel.SelectedVideoSource = source;
            }
            else if (Settings.LastSourceKind == VideoSourceKind.Screen && ScreenItem.Count > 1)
            {
                VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Screen;

                var source = VideoViewModel.AvailableVideoSources.FirstOrDefault(screen => screen.ToString() == Settings.LastSourceName);

                if (source != null)
                    VideoViewModel.SelectedVideoSource = source;
            }
            else if (Settings.LastSourceKind == VideoSourceKind.Region)
            {
                VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Region;
                var rect = (Rectangle)RectangleConverter.ConvertFromString(Settings.LastSourceName);

                VideoViewModel.RegionProvider.SelectedRegion = rect;
            }
            else if (Settings.LastSourceKind == VideoSourceKind.NoVideo)
                VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.NoVideo;
            #endregion

            // Restore Video Codec
            if (VideoViewModel.AvailableVideoWriterKinds.Contains(Settings.LastVideoWriterKind))
            {
                VideoViewModel.SelectedVideoWriterKind = Settings.LastVideoWriterKind;

                var codec = VideoViewModel.AvailableVideoWriters.FirstOrDefault(c => c.ToString() == Settings.LastVideoWriterName);

                if (codec != null)
                    VideoViewModel.SelectedVideoWriter = codec;
            }

            // Restore Audio Codec
            if (!string.IsNullOrEmpty(Settings.LastAudioWriterName))
            {
                var codec = AudioViewModel.AvailableAudioWriters.FirstOrDefault(c => c.ToString() == Settings.LastAudioWriterName);

                if (codec != null)
                    AudioViewModel.SelectedAudioWriter = codec;
            }

            // Restore Microphone
            if (!string.IsNullOrEmpty(Settings.LastMicName))
            {
                var source = AudioViewModel.AudioSource.AvailableRecordingSources.FirstOrDefault(codec => codec.ToString() == Settings.LastMicName);

                if (source != null)
                    AudioViewModel.AudioSource.SelectedRecordingSource = source;
            }

            // Restore Loopback Speaker
            if (!string.IsNullOrEmpty(Settings.LastSpeakerName))
            {
                var source = AudioViewModel.AudioSource.AvailableLoopbackSources.FirstOrDefault(codec => codec.ToString() == Settings.LastSpeakerName);

                if (source != null)
                    AudioViewModel.AudioSource.SelectedLoopbackSource = source;
            }

            // Restore ScreenShot Format
            if (!string.IsNullOrEmpty(Settings.LastScreenShotFormat))
            {
                var format = ScreenShotImageFormats.FirstOrDefault(f => f.ToString() == Settings.LastScreenShotFormat);

                if (format != null)
                    SelectedScreenShotImageFormat = format;
            }
        }

        bool _persist, _hotkeys;

        public void Init(bool Persist, bool Timer, bool Remembered, bool Hotkeys)
        {
            _persist = Persist;
            _hotkeys = Hotkeys;

            if (Timer)
            {
                _timer = new Timer(1000);
                _timer.Elapsed += TimerOnElapsed;
            }

            AudioViewModel.AudioSource.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(AudioViewModel.AudioSource.SelectedRecordingSource):
                    case nameof(AudioViewModel.AudioSource.SelectedLoopbackSource):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            VideoViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(VideoViewModel.SelectedVideoSourceKind):
                    case nameof(VideoViewModel.SelectedVideoSource):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            _cursor = new MouseCursor(Settings.IncludeCursor);

            Settings.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(Settings.IncludeCursor):
                    case null:
                    case "":
                        _cursor.Include = Settings.IncludeCursor;
                        break;
                }
            };

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(Settings.OutPath))
                Settings.OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");

            // Create the Output Directory if it does not exist
            if (!Directory.Exists(Settings.OutPath))
                Directory.CreateDirectory(Settings.OutPath);

            // Register ActionServices
            ServiceProvider.Register<Action>(ServiceName.Recording, () => RecordCommand.ExecuteIfCan());
            ServiceProvider.Register<Action>(ServiceName.Pause, () => PauseCommand.ExecuteIfCan());
            ServiceProvider.Register<Action>(ServiceName.ScreenShot, () => ScreenShotCommand.ExecuteIfCan());
            ServiceProvider.Register<Action>(ServiceName.ActiveScreenShot, () => SaveScreenShot(ScreenShotWindow(Window.ForegroundWindow)));
            ServiceProvider.Register<Action>(ServiceName.DesktopScreenShot, () => SaveScreenShot(ScreenShotWindow(Window.DesktopWindow)));
            ServiceProvider.Register<Func<Window>>(ServiceName.SelectedWindow, () => (VideoViewModel.SelectedVideoSource as WindowItem).Window);

            // Register Hotkeys if not console
            if (_hotkeys)
                HotKeyManager.RegisterAll();

            VideoViewModel.Init();

            if (Remembered)
                RestoreRemembered();

            _webCamProvider = ServiceProvider.Get<IWebCamProvider>(ServiceName.WebCam);

            _webCamProvider.IsVisibleChanged += () => OnPropertyChanged(nameof(WebCamVisible));
        }

        void Remember()
        {
            #region Remember Video Source
            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                case VideoSourceKind.Screen:
                    Settings.LastSourceKind = VideoViewModel.SelectedVideoSourceKind;
                    Settings.LastSourceName = VideoViewModel.SelectedVideoSource.ToString();
                    break;

                case VideoSourceKind.Region:
                    Settings.LastSourceKind = VideoSourceKind.Region;
                    var rect = VideoViewModel.RegionProvider.SelectedRegion;
                    Settings.LastSourceName = RectangleConverter.ConvertToString(rect);
                    break;

                case VideoSourceKind.NoVideo:
                    Settings.LastSourceKind = VideoSourceKind.NoVideo;
                    Settings.LastSourceName = "";
                    break;

                default:
                    Settings.LastSourceKind = VideoSourceKind.Window;
                    Settings.LastSourceName = "";
                    break;
            }
            #endregion

            // Remember Video Codec
            Settings.LastVideoWriterKind = VideoViewModel.SelectedVideoWriterKind;
            Settings.LastVideoWriterName = VideoViewModel.SelectedVideoWriter.ToString();

            // Remember Audio Sources
            Settings.LastMicName = AudioViewModel.AudioSource.SelectedRecordingSource.ToString();
            Settings.LastSpeakerName = AudioViewModel.AudioSource.SelectedLoopbackSource.ToString();

            // Remember Audio Codec
            Settings.LastAudioWriterName = AudioViewModel.SelectedAudioWriter.ToString();

            // Remember ScreenShot Format
            Settings.LastScreenShotFormat = SelectedScreenShotImageFormat.ToString();
        }

        // Call before Exit to free Resources
        public void Dispose()
        {
            if (_hotkeys)
                HotKeyManager.Dispose();

            AudioViewModel.Dispose();

            RecentViewModel.Dispose();

            // Remember things if not console.
            if (_persist)
                Remember();
            
            // Save if not console
            if (_persist)
                Settings.Save();
        }
        
        void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan += _addend;

            // If Capture Duration is set and reached
            if (Duration > 0 && TimeSpan.TotalSeconds >= Duration)
                _syncContext.Post(state => StopRecording(), null);
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = AudioViewModel.AudioSource.AudioAvailable;

            var videoAvailable = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;
            
            RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);

            ScreenShotCommand.RaiseCanExecuteChanged(videoAvailable);
        }
        
        public void SaveScreenShot(Bitmap bmp)
        {
            // Save to Disk or Clipboard
            if (bmp != null)
            {
                if (Settings.ScreenShotSaveTo == "Clipboard")
                {
                    bmp.WriteToClipboard(SelectedScreenShotImageFormat.Equals(ImageFormat.Png));
                    Status.LocalizationKey = nameof(Resources.ImgSavedClipboard);
                }
                else // Save to Disk
                {
                    try
                    {
                        EnsureOutPath();

                        var extension = SelectedScreenShotImageFormat.Equals(ImageFormat.Icon) ? "ico"
                            : SelectedScreenShotImageFormat.Equals(ImageFormat.Jpeg) ? "jpg"
                            : SelectedScreenShotImageFormat.ToString().ToLower();

                        var fileName = Path.Combine(Settings.OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + extension);

                        bmp.Save(fileName, SelectedScreenShotImageFormat);
                        Status.LocalizationKey = nameof(Resources.ImgSavedDisk);
                        RecentViewModel.Add(fileName, RecentItemType.Image, false);

                        ServiceProvider.SystemTray.ShowScreenShotNotification(fileName);
                    }
                    catch (Exception E)
                    {
                        ServiceProvider.Messenger.ShowError(nameof(Resources.NotSaved) + $"\n\n{E}");

                        Status.LocalizationKey = nameof(Resources.NotSaved);
                    }
                }

                bmp.Dispose();
            }
            else Status.LocalizationKey = nameof(Resources.ImgEmpty);
        }

        public Bitmap ScreenShotWindow(Window hWnd)
        {
            ServiceProvider.SystemTray.HideNotification();

            if (hWnd == Window.DesktopWindow)
            {
                var bmp = ScreenShot.Capture(Settings.IncludeCursor);
                
                return TransformedImageProvider.Transform(bmp);
            }
            else
            {
                var bmp = ScreenShot.CaptureTransparent(hWnd, Settings.IncludeCursor,
                         Settings.DoResize, Settings.ResizeWidth, Settings.ResizeHeight);

                // Capture without Transparency
                if (bmp == null)
                    bmp = ScreenShot.Capture(hWnd, Settings.IncludeCursor);

                return TransformedImageProvider.Transform(bmp, true);
            }
        }

        public void CaptureScreenShot()
        {
            ServiceProvider.SystemTray.HideNotification();

            Bitmap bmp = null;

            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = Settings.IncludeCursor;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (selectedVideoSource as WindowItem)?.Window ?? Window.DesktopWindow;

                    bmp = ScreenShotWindow(hWnd);
                    break;

                case VideoSourceKind.Screen:
                    bmp = (selectedVideoSource as ScreenItem)?.Capture(includeCursor);
                    bmp = TransformedImageProvider.Transform(bmp);
                    break;

                case VideoSourceKind.Region:
                    bmp = ScreenShot.Capture(VideoViewModel.RegionProvider.SelectedRegion, includeCursor);
                    bmp = TransformedImageProvider.Transform(bmp);
                    break;
            }

            SaveScreenShot(bmp);
        }

        static void EnsureOutPath()
        {
            if (!Directory.Exists(Settings.Instance.OutPath))
                Directory.CreateDirectory(Settings.Instance.OutPath);
        }

        public void StartRecording()
        {
            VideoViewModel.RegionProvider.SnapEnabled = false;

            ServiceProvider.SystemTray.HideNotification();

            if (Settings.MinimizeOnStart)
                ServiceProvider.Get<Action<bool>>(ServiceName.Minimize).Invoke(true);
            
            CanChangeVideoSource = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Window;

            EnsureOutPath();
            
            if (StartDelay < 0)
                StartDelay = 0;

            if (Duration != 0 && (StartDelay > Duration * 1000))
            {
                Status.LocalizationKey = nameof(Resources.DelayGtDuration);
                SystemSounds.Asterisk.Play();
                return;
            }

            RecorderState = RecorderState.Recording;
            
            isVideo = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;
            
            var extension = isVideo
                ? VideoViewModel.SelectedVideoWriter.Extension
                : AudioViewModel.SelectedAudioWriter.Extension;

            _currentFileName = Path.Combine(Settings.OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status.LocalizationKey = StartDelay > 0 ? nameof(Resources.Waiting) : nameof(Resources.Recording);

            _timer?.Stop();
            TimeSpan = TimeSpan.Zero;
            
            var audioSource = AudioViewModel.AudioSource.GetAudioSource();

            var imgProvider = GetImageProvider();
            
            var videoEncoder = GetVideoFileWriter(imgProvider, audioSource);
            
            if (_recorder == null)
            {
                if (isVideo)
                    _recorder = new Recorder(videoEncoder, imgProvider, Settings.FrameRate, audioSource);

                else _recorder = new Recorder(AudioViewModel.SelectedAudioWriter.GetAudioFileWriter(_currentFileName, audioSource.WaveFormat, Settings.AudioQuality), audioSource);
            }

            _recorder.ErrorOccured += E =>
            {
                StopRecording();

                Status.LocalizationKey = nameof(Resources.ErrorOccured);

                ServiceProvider.Messenger.ShowError($"Error Occured\n\n{E}");
            };
            
            if (StartDelay > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(StartDelay);

                    _recorder.Start();
                });
            }
            else _recorder.Start();

            _timer?.Start();
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider)
        {
            if (VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.NoVideo)
                return null;
            
            IVideoFileWriter videoEncoder = null;
            
            var encoder = VideoViewModel.SelectedVideoWriter.GetVideoFileWriter(_currentFileName, Settings.FrameRate, Settings.VideoQuality, ImgProvider, Settings.AudioQuality, AudioProvider);

            switch (encoder)
            {
                case GifWriter gif:
                    if (Settings.GifVariable)
                        _recorder = new VariableFrameRateGifRecorder(gif, ImgProvider);
                    
                    else videoEncoder = gif;
                    break;

                default:
                    videoEncoder = encoder;
                    break;
            }

            return videoEncoder;
        }
        
        IImageProvider GetImageProvider()
        {
            Func<Point> offset = () => Point.Empty;

            var imageProvider = VideoViewModel.SelectedVideoSource?.GetImageProvider(out offset);

            if (imageProvider == null)
                return null;

            var overlays = new List<IOverlay>
            {
                _cursor
            };

            if (MouseKeyHookAvailable)
                overlays.Add(new MouseKeyHook(Settings.MouseClicks, Settings.KeyStrokes));

            var overlayed = new OverlayedImageProvider(imageProvider, offset, overlays.ToArray());

            return new TransformedImageProvider(overlayed);
        }
        
        public async Task StopRecording()
        {
            Status.LocalizationKey = nameof(Resources.Stopped);

            var savingRecentItem = RecentViewModel.Add(_currentFileName, isVideo ? RecentItemType.Video : RecentItemType.Audio, true);
            
            RecorderState = RecorderState.NotRecording;

            // Set Recorder to null
            var rec = _recorder;
            _recorder = null;

            var task = Task.Run(() => rec.Dispose());

            _timer?.Stop();

            #region After Recording Tasks
            CanChangeVideoSource = true;
            
            if (Settings.MinimizeOnStart)
                ServiceProvider.Get<Action<bool>>(ServiceName.Minimize).Invoke(false);

            VideoViewModel.RegionProvider.SnapEnabled = true;
            #endregion

            // Ensure saved
            await task;
            
            // After Save
            savingRecentItem.Saved();
            
            ServiceProvider.SystemTray.ShowTextNotification((isVideo ? Resources.VideoSaved : Resources.AudioSaved) + ": " + Path.GetFileName(_currentFileName), 5000, () =>
            {
                ServiceProvider.LaunchFile(new ProcessStartInfo(_currentFileName));
            });
        }
    }
}