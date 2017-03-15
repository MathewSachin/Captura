using Screna;
using Screna.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using Window = Screna.Window;

namespace Captura
{
    public partial class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        readonly Timer _timer;
        IRecorder _recorder;
        string _currentFileName;
        readonly MouseCursor _cursor;
        bool isVideo;
        #endregion

        MainViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;

            #region Commands
            ScreenShotCommand = new DelegateCommand(CaptureScreenShot);

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

                AudioViewModel.RefreshAudioSources();

                Status = "Refreshed";
            });

            OpenOutputFolderCommand = new DelegateCommand(() => 
            {
                EnsureOutPath();

                Process.Start("explorer.exe", Settings.OutPath);
            });

            PauseCommand = new DelegateCommand(() =>
            {
                if (RecorderState == RecorderState.Paused)
                {
                    _recorder.Start();
                    _timer.Start();

                    RecorderState = RecorderState.Recording;
                    Status = "Recording...";
                }
                else
                {
                    _recorder.Stop();
                    _timer.Stop();

                    RecorderState = RecorderState.Paused;
                    Status = "Paused";

                    SystemTrayManager.ShowNotification("Recording Paused", " ", 500, null);
                }
            }, false);

            SelectOutputFolderCommand = new DelegateCommand(() =>
            {
                var dlg = new FolderBrowserDialog
                {
                    SelectedPath = Settings.OutPath,
                    Description = "Select Output Folder"
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.OutPath = dlg.SelectedPath;
            });
            #endregion

            //Populate Available Codecs, Audio and Video Sources ComboBoxes
            RefreshCommand.Execute(null);

            AudioViewModel.PropertyChanged += (Sender, Args) =>
            {
                if (Args.PropertyName == nameof(AudioViewModel.SelectedRecordingSource)
                || Args.PropertyName == nameof(AudioViewModel.SelectedLoopbackSource))
                    CheckFunctionalityAvailability();
            };

            VideoViewModel.PropertyChanged += (Sender, Args) =>
            {
                if (Args.PropertyName == nameof(VideoViewModel.SelectedVideoSource))
                    CheckFunctionalityAvailability();
            };

            _cursor = new MouseCursor(Settings.IncludeCursor);

            Settings.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(Settings.IncludeCursor):
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
            
            HotKeyManager.RegisterAll();
            SystemTrayManager.Init();
        }

        // Call before Exit to free Resources
        public void Dispose()
        {
            HotKeyManager.Dispose();
            SystemTrayManager.Dispose();

            AudioViewModel.Dispose();

            Settings.Save();
        }
        
        void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan += _addend;

            // If Capture Duration is set and reached
            if (Duration > 0 && TimeSpan.TotalSeconds >= Duration)
                StopRecording();
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = AudioViewModel.SelectedRecordingSource != null || AudioViewModel.SelectedLoopbackSource != null;

            var videoAvailable = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;
            
            RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);

            ScreenShotCommand.RaiseCanExecuteChanged(videoAvailable);
        }
        
        void CaptureScreenShot()
        {
            EnsureOutPath();

            string fileName = null;
            
            var extension = SelectedScreenShotImageFormat.Equals(ImageFormat.Icon) ? "ico"
                : SelectedScreenShotImageFormat.Equals(ImageFormat.Jpeg) ? "jpg"
                : SelectedScreenShotImageFormat.ToString().ToLower();

            var saveToClipboard = SelectedScreenShotSaveTo == "Clipboard";

            if (!saveToClipboard)
                fileName = Path.Combine(Settings.OutPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + extension);

            Bitmap bmp = null;

            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = Settings.IncludeCursor;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (selectedVideoSource as WindowItem)?.Window ?? Window.DesktopWindow;

                    if (hWnd == Window.DesktopWindow)
                        bmp = ScreenShot.Capture(includeCursor);
                    else
                    {
                        bmp = ScreenShot.CaptureTransparent(hWnd, includeCursor,
                                 ScreenShotDoResize, ScreenShotResizeWidth, ScreenShotResizeHeight);

                        // Capture without Transparency
                        if (bmp == null)
                            bmp = ScreenShot.Capture(hWnd, includeCursor);
                    }
                    break;

                case VideoSourceKind.Screen:
                    bmp = (selectedVideoSource as ScreenItem)?.Capture(includeCursor);
                    break;

                case VideoSourceKind.Region:
                    bmp = ScreenShot.Capture(RegionSelector.Instance.Rectangle, includeCursor);
                    break;
            }

            // Save to Disk or Clipboard
            if (bmp != null)
            {
                if (saveToClipboard)
                {
                    bmp.WriteToClipboard(SelectedScreenShotImageFormat.Equals(ImageFormat.Png));
                    Status = "Image Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        bmp.Save(fileName, SelectedScreenShotImageFormat);
                        Status = "Image Saved to Disk";
                        RecentViewModel.Add(fileName, RecentItemType.Image);

                        SystemTrayManager.ShowNotification("ScreenShot Saved", Path.GetFileName(fileName), 3000, () => Process.Start(fileName));
                    }
                    catch (Exception E)
                    {
                        Status = "Not Saved. " + E.Message;
                    }
                }

                bmp.Dispose();
            }
            else Status = "Not Saved - Image taken was Empty";
        }

        void EnsureOutPath()
        {
            if (!Directory.Exists(Settings.OutPath))
                Directory.CreateDirectory(Settings.OutPath);
        }

        void StartRecording()
        {
            if (Settings.MinimizeOnStart)
                WindowState = WindowState.Minimized;
            
            CanChangeVideoSource = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Window;

            EnsureOutPath();
            
            if (StartDelay < 0)
                StartDelay = 0;

            if (Duration != 0 && (StartDelay * 1000 > Duration))
            {
                Status = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            RecorderState = RecorderState.Recording;
            
            isVideo = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;
            
            var extension = isVideo
                ? VideoViewModel.SelectedVideoWriter.Extension
                : AudioViewModel.SelectedAudioWriter.Extension;

            _currentFileName = Path.Combine(Settings.OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status = StartDelay > 0 ? $"Recording from t = {StartDelay} ms..." : "Recording...";

            _timer.Stop();
            TimeSpan = TimeSpan.Zero;
            
            var audioSource = AudioViewModel.GetAudioSource();

            var imgProvider = GetImageProvider();
            
            var videoEncoder = GetVideoFileWriter(imgProvider, audioSource);
            
            if (_recorder == null)
            {
                if (isVideo)
                    _recorder = new Recorder(videoEncoder, imgProvider, Settings.FrameRate, audioSource);

                else _recorder = new Recorder(AudioViewModel.SelectedAudioWriter.GetAudioFileWriter(_currentFileName, audioSource.WaveFormat, Settings.AudioQuality), audioSource);
            }

            /*_recorder.RecordingStopped += (s, E) =>
            {
                OnStopped();

                if (E?.Error == null)
                    return;

                Status = "Error";
                MessageBox.Show(E.ToString());
            };*/
            
            if (StartDelay > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(StartDelay);

                    _recorder.Start();
                });
            }
            else _recorder.Start();

            _timer.Start();
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
                    if (Settings.GifUnconstrained)
                        _recorder = new UnconstrainedFrameRateGifRecorder(gif, ImgProvider);
                    
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
            Func<System.Drawing.Point> offset = () => System.Drawing.Point.Empty;

            var imageProvider = VideoViewModel.SelectedVideoSource?.GetImageProvider(out offset);

            if (imageProvider == null)
                return null;

            var overlays = new List<IOverlay> { _cursor };

            if (MouseKeyHookAvailable)
                overlays.Add(new MouseKeyHook(Settings.MouseClicks, Settings.KeyStrokes));

            return new OverlayedImageProvider(imageProvider, offset, overlays.ToArray());
        }
        
        async void StopRecording()
        {
            Status = "Stopped";

            var savingRecentItem = RecentViewModel.AddTemp(_currentFileName);
            
            RecorderState = RecorderState.NotRecording;

            CanChangeVideoSource = true;
            
            if (Settings.MinimizeOnStart)
                WindowState = WindowState.Normal;

            _timer.Stop();

            var rec = _recorder;
            _recorder = null;

            await Task.Run(() => rec.Dispose());

            // After Save
            RecentViewModel.RecentList.Remove(savingRecentItem);
            RecentViewModel.Add(_currentFileName, isVideo ? RecentItemType.Video : RecentItemType.Audio);

            SystemTrayManager.ShowNotification($"{(isVideo ? "Video" : "Audio")} Saved", Path.GetFileName(_currentFileName), 3000, () => Process.Start(_currentFileName));
        }
    }
}