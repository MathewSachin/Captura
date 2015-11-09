// Adopted from SharpAvi Screencast Sample by Vasilli Masillov
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Captura
{
    class Recorder
    {
        #region Public Static
        public static readonly int DesktopHeight, DesktopWidth;

        public static readonly RECT DesktopRectangle;

        public static readonly IntPtr DesktopHandle = User32.GetDesktopWindow();

        static Recorder()
        {
            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            DesktopHeight = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight * toDevice.M22);
            DesktopWidth = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenWidth * toDevice.M11);

            DesktopRectangle = new RECT(0, 0, DesktopWidth, DesktopHeight);
        }

        public static readonly FourCC GifFourCC = new FourCC("_gif");
        #endregion

        static Rectangle CreateRectangle(RECT r) { return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top); }

        #region Fields
        AviWriter AviWriter;
        IAviVideoStream VideoStream;
        IAviAudioStream AudioStream;

        NAudioFacade NAudioFacade = null;

        Thread ScreenThread;
        string FileName;
        int FramesPerSecond, Quality;
        FourCC Codec;
        bool CaptureVideo;

        bool EncodeMp3;

        ManualResetEvent StopCapturing = new ManualResetEvent(false),
            ContinueCapturing = new ManualResetEvent(false);
        AutoResetEvent VideoFrameWritten = new AutoResetEvent(false),
            AudioBlockWritten = new AutoResetEvent(false);

        GifWriter GifWriter;
        bool IsGif { get { return Codec == GifFourCC; } }

        Color BackgroundColor;

        Func<bool> IncludeCursor;
        Func<IntPtr> hWnd;

        MouseKeyHookFacade MouseKeyHookFacade = null;
        #endregion

        #region Create
        IAviVideoStream CreateVideoStream()
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
                return AviWriter.AddUncompressedVideoStream(DesktopWidth, DesktopHeight);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return AviWriter.AddMotionJpegVideoStream(DesktopWidth, DesktopHeight, Quality);
            else
            {
                return AviWriter.AddMpeg4VideoStream(DesktopWidth, DesktopHeight,
                    (double)AviWriter.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }

        IAviAudioStream CreateAudioStream()
        {
            // Create encoding or simple stream based on settings
            if (NAudioFacade.IsLoopback)
                return AviWriter.AddAudioStream(NAudioFacade.Channels,
                    NAudioFacade.SampleRate, NAudioFacade.BitsPerSample, AudioFormats.Float);

            // LAME DLL path is set in App.OnStartup()
            else if (EncodeMp3)
                return AviWriter.AddMp3AudioStream(NAudioFacade.Channels,
                    NAudioFacade.SampleRate, NAudioFacade.AudioBitRate);

            else return AviWriter.AddAudioStream(NAudioFacade.Channels,
                NAudioFacade.SampleRate, NAudioFacade.BitsPerSample);
        }
        #endregion

        public event Action<Exception> Error;

        public Recorder(string FileName, int FramesPerSecond, FourCC Codec, int VideoQuality, string AudioSourceId, bool StereoAudio,
            bool EncodeMp3, int AudioBitRate, bool CaptureMouseClicks, bool CaptureKeyStrokes, Color BackgroundColor,
            Func<bool> IncludeCursor, Func<IntPtr> hWnd)
        {
            this.FileName = FileName;
            this.FramesPerSecond = FramesPerSecond;
            this.Codec = Codec;
            this.Quality = VideoQuality;
            this.EncodeMp3 = EncodeMp3;

            if (AudioSourceId != "-1") NAudioFacade = new NAudioFacade(AudioSourceId, StereoAudio, AudioBitRate);

            this.IncludeCursor = IncludeCursor;
            this.hWnd = hWnd;

            if (CaptureMouseClicks || CaptureKeyStrokes)
                MouseKeyHookFacade = new MouseKeyHookFacade(CaptureMouseClicks, CaptureKeyStrokes);

            this.BackgroundColor = BackgroundColor;

            this.CaptureVideo = hWnd().ToInt32() != -1 && Codec != Recorder.GifFourCC;
            
            InitVideo();

            // Not Actually Started, Waits for ContinueThread to be Set
            if (ScreenThread != null) ScreenThread.Start();
        }

        #region Init
        void InitAudio()
        {
            if (NAudioFacade.IsLoopback)
            {
                NAudioFacade.InitSilencePlayer();

                if (CaptureVideo)
                {
                    AudioStream = CreateAudioStream();
                    AudioStream.Name = "Loopback";
                }

                NAudioFacade.InitLoopback();
            }
            else
            {
                if (CaptureVideo)
                {
                    AudioStream = CreateAudioStream();
                    AudioStream.Name = "Voice";
                }

                NAudioFacade.InitRecording(FramesPerSecond);
            }

            if (IsGif || !CaptureVideo)
                NAudioFacade.CreateWaveWriter(IsGif
                    ? Path.ChangeExtension(FileName, "wav")
                    : FileName);

            NAudioFacade.DataAvailable += AudioDataAvailable;
        }

        void InitVideo()
        {
            if (CaptureVideo)
            {
                AviWriter = new AviWriter(FileName)
                {
                    FramesPerSecond = FramesPerSecond,
                    EmitIndex1 = true,
                };

                VideoStream = CreateVideoStream();
                VideoStream.Name = "Captura";
            }
            else if (IsGif) GifWriter = new GifWriter(FileName, 1000 / FramesPerSecond, 1);

            if (NAudioFacade != null) InitAudio();

            if (CaptureVideo || IsGif)
            {
                ScreenThread = new Thread(RecordScreen)
                {
                    Name = "Captura.RecordScreen",
                    IsBackground = true
                };
            }
        }
        #endregion

        #region Control
        public void Start(int Delay = 0)
        {
            new Thread(new ParameterizedThreadStart((e) =>
                {
                    try
                    {
                        Thread.Sleep((int)e);                        

                        if (CaptureVideo || IsGif) ContinueCapturing.Set();

                        if (NAudioFacade != null)
                        {
                            NAudioFacade.PlaySilence();

                            if (CaptureVideo)
                            {
                                VideoFrameWritten.Set();
                                AudioBlockWritten.Reset();
                            }

                            NAudioFacade.StartRecording();
                        }
                    }
                    catch (Exception E) { if (Error != null) Error(E); }
                })).Start(Delay);
        }

        public void Pause()
        {            
            if (CaptureVideo || IsGif) ContinueCapturing.Reset();

            if (NAudioFacade != null)
            {
                NAudioFacade.StopRecording();

                NAudioFacade.PauseSilence();
            }
        }

        public void Stop()
        {
            // Resume if Paused
            ContinueCapturing.Set();

            if (MouseKeyHookFacade != null) MouseKeyHookFacade.Dispose();

            // Video
            if (CaptureVideo || IsGif)
            {
                if (StopCapturing != null && !StopCapturing.SafeWaitHandle.IsClosed)
                    StopCapturing.Set();

                if (!ScreenThread.Join(500)) ScreenThread.Abort();

                ScreenThread = null;
            }

            // Audio Source
            if (NAudioFacade != null)
            {
                NAudioFacade.Dispose();
                NAudioFacade = null;
            }

            // WaitHandles
            if (StopCapturing != null && !StopCapturing.SafeWaitHandle.IsClosed)
            {
                StopCapturing.Dispose();
                StopCapturing = null;
            }

            if (ContinueCapturing != null && !ContinueCapturing.SafeWaitHandle.IsClosed)
            {
                ContinueCapturing.Dispose();
                ContinueCapturing = null;
            }

            // Writers
            if (AviWriter != null)
            {
                AviWriter.Close();
                AviWriter = null;
            }

            if (GifWriter != null)
            {
                GifWriter.Dispose();
                GifWriter = null;
            }            
        }
        #endregion

        #region Background
        void RecordScreen()
        {
            try
            {
                var FrameInterval = TimeSpan.FromSeconds(1 / (double)FramesPerSecond);
                Task FrameWriteTask = null;
                var TimeTillNextFrame = TimeSpan.Zero;

                byte[] Buffer = null;
                Image ScreenshotImage = null;

                if (CaptureVideo) Buffer = new byte[DesktopWidth * DesktopHeight * 4];

                ContinueCapturing.WaitOne();

                while (!StopCapturing.WaitOne(TimeTillNextFrame))
                {
                    var Timestamp = DateTime.Now;

                    if (IsGif) ScreenshotImage = ScreenShot(hWnd(), IncludeCursor());
                    else ScreenShot(Buffer);

                    // Wait for the previous frame is written
                    if (FrameWriteTask != null)
                    {
                        FrameWriteTask.Wait();
                        if (!IsGif) VideoFrameWritten.Set();
                    }

                    if (!IsGif && AudioStream != null && !NAudioFacade.IsLoopback)
                        if (WaitHandle.WaitAny(new WaitHandle[] { AudioBlockWritten, StopCapturing }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    FrameWriteTask = IsGif ? GifWriter.WriteFrameAsync(ScreenshotImage)
                        : VideoStream.WriteFrameAsync(true, Buffer, 0, Buffer.Length);

                    TimeTillNextFrame = Timestamp + FrameInterval - DateTime.Now;
                    if (TimeTillNextFrame < TimeSpan.Zero) TimeTillNextFrame = TimeSpan.Zero;

                    ContinueCapturing.WaitOne();
                }

                // Wait for the last frame is written
                if (FrameWriteTask != null) FrameWriteTask.Wait();
            }
            catch (Exception E)
            {
                Stop();
                if (Error != null) Error.Invoke(E);
            }
        }

        public Bitmap ScreenShot(IntPtr hWnd, bool IncludeCursor)
        {
            RECT Rect = DesktopRectangle;

            if (hWnd != DesktopHandle)
                User32.GetWindowRect(hWnd, ref Rect);

            var BMP = new Bitmap(DesktopWidth, DesktopHeight);

            using (var g = Graphics.FromImage(BMP))
            {
                if (BackgroundColor != Color.Transparent)
                    g.FillRectangle(new SolidBrush(BackgroundColor), CreateRectangle(DesktopRectangle));

                g.CopyFromScreen(Rect.Left, Rect.Top, Rect.Left, Rect.Top,
                    new System.Drawing.Size(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top),
                    CopyPixelOperation.SourceCopy);

                #region Include Cursor
                if (IncludeCursor)
                {
                    int CursorX = 0, CursorY = 0;

                    IntPtr hIcon = IntPtr.Zero;
                    CursorInfo ci = new CursorInfo() { cbSize = Marshal.SizeOf(typeof(CursorInfo)) };

                    IconInfo icInfo;

                    if (User32.GetCursorInfo(out ci))
                    {
                        if (ci.flags == User32.CURSOR_SHOWING)
                        {
                            hIcon = User32.CopyIcon(ci.hCursor);
                            if (User32.GetIconInfo(hIcon, out icInfo))
                            {
                                CursorX = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                                CursorY = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);
                            }
                        }
                    }

                    if (hIcon != IntPtr.Zero)
                    {
                        Bitmap CursorBMP = Icon.FromHandle(hIcon).ToBitmap();
                        g.DrawImage(CursorBMP, CursorX, CursorY, CursorBMP.Width, CursorBMP.Height);
                    }
                }
                #endregion

                if (MouseKeyHookFacade != null) MouseKeyHookFacade.Draw(g);

                g.Flush();
            }

            return BMP;
        }

        public void ScreenShot(byte[] Buffer)
        {
            using (var BMP = ScreenShot(hWnd(), IncludeCursor()))
            {
                var bits = BMP.LockBits(CreateRectangle(DesktopRectangle), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                BMP.UnlockBits(bits);
            }
        }

        void AudioDataAvailable(byte[] Buffer, int BytesRecorded)
        {
            if (CaptureVideo)
            {
                if (NAudioFacade.IsLoopback) AudioStream.WriteBlock(Buffer, 0, BytesRecorded);
                else
                {
                    var signalled = WaitHandle.WaitAny(new WaitHandle[] { VideoFrameWritten, StopCapturing });
                    if (signalled == 0)
                    {
                        AudioStream.WriteBlock(Buffer, 0, BytesRecorded);

                        AudioBlockWritten.Set();
                    }
                }
            }
            else NAudioFacade.WaveWrite(Buffer, BytesRecorded);
        }
        #endregion
    }
}
