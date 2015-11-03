// Adopted from SharpAvi Screencast Sample by Vasilli Masillov
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using ManagedWin32.Api;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Captura
{
    class Recorder : IDisposable
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

        public static bool MouseClicked = false;
        public static Keys LastKeyPressed = Keys.None;

        public static readonly FourCC GifFourCC = new FourCC("_gif");
        #endregion

        static Rectangle CreateRectangle(RECT r)
        {
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        #region Fields
        AviWriter AviWriter;
        IAviVideoStream VideoStream;
        IAviAudioStream AudioStream;

        IWaveIn AudioSource;
        WaveFileWriter WaveWriter;
        IWavePlayer SilencePlayer;

        Thread ScreenThread;
        ManualResetEvent StopThread = new ManualResetEvent(false);
        AutoResetEvent VideoFrameWritten = new AutoResetEvent(false),
            AudioBlockWritten = new AutoResetEvent(false);
        
        public bool IsPaused = false;
        
        GifWriter GifWriter;
        Color BackgroundColor;

        MMDevice LoopbackDevice { get { return new MMDeviceEnumerator().GetDevice(AudioSourceId); } }

        bool IsGif { get { return Codec == GifFourCC; } }

        string FileName, AudioSourceId;
        int FramesPerSecond, Quality, AudioBitRate;
        FourCC Codec;
        bool EncodeAudio, CaptureVideo, CaptureMouseClicks, CaptureKeyStrokes, IsLoopback;

        Func<bool> IncludeCursor;
        Func<IntPtr> hWnd;

        WaveFormat WaveFormat;
        #endregion

        #region Create
        AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        GifWriter CreateGifWriter()
        {
            return new GifWriter(FileName, 1000 / FramesPerSecond, 1);
        }

        IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
                return writer.AddUncompressedVideoStream(DesktopWidth, DesktopHeight);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(DesktopWidth, DesktopHeight, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(DesktopWidth, DesktopHeight,
                    (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }

        IAviAudioStream CreateAudioStream(AviWriter writer)
        {
            // Create encoding or simple stream based on settings
            if (IsLoopback)
                return writer.AddAudioStream(WaveFormat.Channels,
                    WaveFormat.SampleRate, WaveFormat.BitsPerSample, AudioFormats.Float);

            // LAME DLL path is set in App.OnStartup()
            else if (EncodeAudio)
                return writer.AddMp3AudioStream(WaveFormat.Channels,
                    WaveFormat.SampleRate, AudioBitRate);

            else return writer.AddAudioStream(WaveFormat.Channels,
                WaveFormat.SampleRate, WaveFormat.BitsPerSample);
        }

        WaveFileWriter CreateWaveWriter()
        {
            return new WaveFileWriter(IsGif
                ? Path.ChangeExtension(FileName, "wav")
                : FileName, WaveFormat);
        }
        #endregion

        public event Action<Exception> Error;

        #region Factory
        public Recorder(string FileName, int FramesPerSecond, FourCC Codec, int VideoQuality, string AudioSourceId, bool StereoAudio,
            bool EncodeMp3, int AudioBitRate, bool CaptureMouseClicks, bool CaptureKeyStrokes, Color BackgroundColor,
            Func<bool> IncludeCursor, Func<IntPtr> hWnd)
        {
            this.FileName = FileName;
            this.FramesPerSecond = FramesPerSecond;
            this.Codec = Codec;
            this.Quality = VideoQuality;
            this.EncodeAudio = EncodeMp3;
            this.AudioBitRate = AudioBitRate;
            this.AudioSourceId = AudioSourceId;

            this.IncludeCursor = IncludeCursor;
            this.hWnd = hWnd;

            this.CaptureMouseClicks = CaptureMouseClicks;
            this.CaptureKeyStrokes = CaptureKeyStrokes;
            this.BackgroundColor = BackgroundColor;

            int val;
            IsLoopback = !int.TryParse(AudioSourceId, out val);

            this.CaptureVideo = hWnd().ToInt32() != -1 && Codec != Recorder.GifFourCC;

            WaveFormat = IsLoopback ? LoopbackDevice.AudioClient.MixFormat : new WaveFormat(44100, 16, StereoAudio ? 2 : 1);
        }

        public void Dispose()
        {
            if (IsPaused) Resume();

            if (SilencePlayer != null)
            {
                SilencePlayer.Stop();
                SilencePlayer.Dispose();
                SilencePlayer = null;
            }

            if (CaptureVideo || IsGif)
            {
                if (!StopThread.SafeWaitHandle.IsClosed
                    && !StopThread.SafeWaitHandle.IsInvalid)
                    StopThread.Set();
                ScreenThread.Join(500);
                if (ScreenThread.IsAlive) ScreenThread.Abort();
            }

            if (AudioSource != null)
            {
                AudioSource.StopRecording();
                AudioSource.DataAvailable -= AudioDataAvailable;
            }

            if (CaptureVideo)
            {
                // Close writer: the remaining data is written to a file and file is closed
                AviWriter.Close();

                if (!StopThread.SafeWaitHandle.IsClosed) StopThread.Close();
            }
            else if (IsGif)
            {
                GifWriter.Dispose();
                if (!StopThread.SafeWaitHandle.IsClosed) StopThread.Close();
                if (WaveWriter != null) WaveWriter.Dispose();
            }
            else WaveWriter.Dispose();
        }
        #endregion

        #region Init
        void InitGif()
        {
            GifWriter = CreateGifWriter();

            InitAudio();

            ScreenThread = new Thread(RecordScreenAsGif)
            {
                Name = typeof(GifWriter).Name + ".RecordScreen",
                IsBackground = true
            };

            ScreenThread.Start();

            if (AudioSource != null)
            {
                WaveWriter = CreateWaveWriter();

                AudioSource.DataAvailable += AudioDataAvailable;

                AudioSource.StartRecording();
            }
        }

        void InitAudio()
        {
            if (IsLoopback)
            {
                var dev = LoopbackDevice;

                SilencePlayer = new WasapiOut(dev, AudioClientShareMode.Shared, false, 100);

                SilencePlayer.Init(new SilenceProvider(WaveFormat));

                SilencePlayer.Play();

                if (CaptureVideo)
                {
                    AudioStream = CreateAudioStream(AviWriter);
                    AudioStream.Name = "Loopback";
                }

                AudioSource = new WasapiLoopbackCapture(dev) { ShareMode = AudioClientShareMode.Shared };
            }
            else
            {
                int Id = int.Parse(AudioSourceId);

                if (Id != -1)
                {
                    if (CaptureVideo)
                    {
                        AudioStream = CreateAudioStream(AviWriter);
                        AudioStream.Name = "Voice";
                    }

                    AudioSource = new WaveInEvent
                    {
                        DeviceNumber = Id,
                        WaveFormat = WaveFormat,
                        // Buffer size to store duration of 1 frame
                        BufferMilliseconds = (int)Math.Ceiling(1000 / (decimal)FramesPerSecond),
                        NumberOfBuffers = 3,
                    };
                }
            }
        }

        void InitSharpAvi()
        {
            if (CaptureVideo)
            {
                // Create AVI writer and specify FPS
                AviWriter = CreateAviWriter();

                // Create video stream
                VideoStream = CreateVideoStream(AviWriter);
                // Set only name. Other properties were when creating stream, 
                // either explicitly by arguments or implicitly by the encoder used
                VideoStream.Name = "Captura";
            }

            InitAudio();

            if (CaptureVideo)
            {
                ScreenThread = new Thread(RecordScreen)
                {
                    Name = typeof(Recorder).Name + ".RecordScreen",
                    IsBackground = true
                };
            }
            else WaveWriter = CreateWaveWriter();

            if (CaptureVideo) ScreenThread.Start();

            if (AudioSource != null)
            {
                AudioSource.DataAvailable += AudioDataAvailable;

                if (CaptureVideo)
                {
                    VideoFrameWritten.Set();
                    AudioBlockWritten.Reset();
                }

                AudioSource.StartRecording();
            }
        }
        #endregion

        #region Control
        public void Start(int Delay)
        {
            new Thread(new ParameterizedThreadStart((e) =>
                {
                    try
                    {
                        Thread.Sleep((int)e);

                        if (!IsGif) InitSharpAvi();
                        else InitGif();
                    }
                    catch (Exception E) { if (Error != null) Error(E); }
                })).Start(Delay);
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                if (SilencePlayer != null) SilencePlayer.Pause();

                if (CaptureVideo || IsGif) ScreenThread.Suspend();

                if (AudioSource != null) AudioSource.StopRecording();

                IsPaused = true;
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                if (SilencePlayer != null) SilencePlayer.Play();

                if (CaptureVideo || IsGif) ScreenThread.Resume();

                if (AudioSource != null)
                {
                    if (CaptureVideo)
                    {
                        VideoFrameWritten.Set();
                        AudioBlockWritten.Reset();
                    }
                    AudioSource.StartRecording();
                }

                IsPaused = false;
            }
        }
        #endregion

        #region Background
        void RecordScreen()
        {
            try
            {
                var FrameInterval = TimeSpan.FromSeconds(1 / (double)AviWriter.FramesPerSecond);
                var Buffer = new byte[DesktopWidth * DesktopHeight * 4];
                Task VideoWriteTask = null;
                var TimeTillNextFrame = TimeSpan.Zero;

                while (!StopThread.WaitOne(TimeTillNextFrame))
                {
                    var Timestamp = DateTime.Now;

                    ScreenShot(Buffer);

                    // Wait for the previous frame is written
                    if (VideoWriteTask != null)
                    {
                        VideoWriteTask.Wait();
                        VideoFrameWritten.Set();
                    }

                    if (AudioStream != null && !IsLoopback)
                        if (WaitHandle.WaitAny(new WaitHandle[] { AudioBlockWritten, StopThread }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    VideoWriteTask = VideoStream.WriteFrameAsync(true, Buffer, 0, Buffer.Length);

                    TimeTillNextFrame = Timestamp + FrameInterval - DateTime.Now;
                    if (TimeTillNextFrame < TimeSpan.Zero) TimeTillNextFrame = TimeSpan.Zero;
                }

                // Wait for the last frame is written
                if (VideoWriteTask != null) VideoWriteTask.Wait();
            }
            catch (Exception E)
            {
                Dispose();
                if (Error != null) Error.Invoke(E);
            }
        }

        void RecordScreenAsGif()
        {
            try
            {
                var FrameInterval = TimeSpan.FromMilliseconds(GifWriter.DefaultFrameDelay);
                var TimeTillNextFrame = TimeSpan.Zero;
                Task GifWriteTask = null;
                Image ScreenshotImage;

                while (!StopThread.WaitOne(TimeTillNextFrame))
                {
                    var Timestamp = DateTime.Now;

                    ScreenshotImage = ScreenShot(hWnd(), IncludeCursor(), true, BackgroundColor, CaptureMouseClicks, CaptureKeyStrokes);

                    if (GifWriteTask != null) GifWriteTask.Wait();

                    if (AudioStream != null && !IsLoopback)
                        if (WaitHandle.WaitAny(new WaitHandle[] { AudioBlockWritten, StopThread }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    GifWriteTask = GifWriter.WriteFrameAsync(ScreenshotImage);

                    TimeTillNextFrame = Timestamp + FrameInterval - DateTime.Now;
                    if (TimeTillNextFrame < TimeSpan.Zero) TimeTillNextFrame = TimeSpan.Zero;
                }

                // Wait for the last frame is written
                if (GifWriteTask != null) GifWriteTask.Wait();
            }
            catch (Exception E)
            {
                Dispose();
                if (Error != null) Error.Invoke(E);
            }
        }

        public static Bitmap ScreenShot(IntPtr hWnd, bool IncludeCursor, bool ScreenCasting, Color BgColor,
            bool CaptureMouseClicks = false, bool CaptureKeyStrokes = false)
        {
            int CursorX = 0, CursorY = 0;
            
            RECT Rect = DesktopRectangle;

            if (hWnd != DesktopHandle)
            {
                User32.GetWindowRect(hWnd, ref Rect);

                if (!ScreenCasting) User32.SetWindowPos(hWnd, (IntPtr)(-1), 0, 0, 0, 0, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoSize);
            }

            var BMP = new Bitmap(DesktopWidth, DesktopHeight);

            using (var g = Graphics.FromImage(BMP))
            {
                if (BgColor != Color.Transparent) g.FillRectangle(new SolidBrush(BgColor), CreateRectangle(DesktopRectangle));

                g.CopyFromScreen(Rect.Left, Rect.Top, Rect.Left, Rect.Top, 
                    new System.Drawing.Size(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top), 
                    CopyPixelOperation.SourceCopy);
                
                #region Include Cursor
                if (IncludeCursor)
                {
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

                #region MouseClicks
                if (ScreenCasting && CaptureMouseClicks && MouseClicked)
                {
                    var curPos = User32.CursorPosition;
                    g.DrawArc(new Pen(Color.Black, 1), curPos.X - 40, curPos.Y - 40, 80, 80, 0, 360);

                    MouseClicked = false;
                }
                #endregion

                #region KeyStrokes
                if (ScreenCasting && CaptureKeyStrokes
                    && LastKeyPressed != System.Windows.Forms.Keys.None)
                {
                    g.DrawString(LastKeyPressed.ToString(),
                        new Font(FontFamily.GenericMonospace, 100),
                        new SolidBrush(Color.Black), 100, 100);

                    LastKeyPressed = System.Windows.Forms.Keys.None;
                }
                #endregion

                g.Flush();
            }

            if (!ScreenCasting) User32.SetWindowPos(hWnd, (IntPtr)(-2), 0, 0, 0, 0, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoSize);

            return BMP;
        }

        public void ScreenShot(byte[] Buffer)
        {
            using (var BMP = ScreenShot(hWnd(), IncludeCursor(), true, BackgroundColor, CaptureMouseClicks, CaptureKeyStrokes))
            {
                var bits = BMP.LockBits(CreateRectangle(DesktopRectangle), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                BMP.UnlockBits(bits);
            }
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e)
        {
            if (CaptureVideo)
            {
                if (IsLoopback) AudioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);
                else
                {
                    var signalled = WaitHandle.WaitAny(new WaitHandle[] { VideoFrameWritten, StopThread });
                    if (signalled == 0)
                    {
                        AudioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);

                        AudioBlockWritten.Set();
                    }
                }
            }
            else WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }
        #endregion
    }
}
