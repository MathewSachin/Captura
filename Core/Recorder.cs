// Adopted from SharpAvi Screencast Sample by Vasilli Masillov
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        #region Fields
        AviWriter writer;
        IAviVideoStream videoStream;
        IAviAudioStream audioStream;
        IWaveIn audioSource;
        Thread screenThread;
        ManualResetEvent stopThread = new ManualResetEvent(false);
        AutoResetEvent videoFrameWritten = new AutoResetEvent(false),
            audioBlockWritten = new AutoResetEvent(false);
        public bool IsPaused = false;
        WaveFileWriter WaveWriter;
        IWavePlayer SilencePlayer;
        GifWriter GifWriter;
        #endregion

        #region Params
        Color BgColor;

        MMDevice LoopbackDevice { get { return new MMDeviceEnumerator().GetDevice(AudioSourceId); } }

        bool IsGif { get { return Codec == Commons.GifFourCC; } }

        string FileName, AudioSourceId;
        int FramesPerSecond, Quality, AudioBitRate;
        FourCC Codec;
        bool EncodeAudio, CaptureVideo, CaptureMouseClicks, CaptureKeyStrokes;

        bool IsLoopback;

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
                return writer.AddUncompressedVideoStream(Commons.DesktopWidth, Commons.DesktopHeight);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(Commons.DesktopWidth, Commons.DesktopHeight, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(Commons.DesktopWidth, Commons.DesktopHeight,
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

        WaveFormat WaveFormat;
        #endregion

        Func<bool> IncludeCursor;
        Func<IntPtr> hWnd;

        public Recorder(string FileName, int FramesPerSecond, FourCC Codec, int VideoQuality, string AudioSourceId, bool StereoAudio,
            bool EncodeMp3, int AudioBitRate, bool CaptureMouseClicks, bool CaptureKeyStrokes, Color BgColor,
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
            this.BgColor = BgColor;

            int val;
            IsLoopback = !int.TryParse(AudioSourceId, out val);

            this.CaptureVideo = hWnd().ToInt32() != -1 && Codec != Commons.GifFourCC;

            WaveFormat = IsLoopback ? LoopbackDevice.AudioClient.MixFormat : new WaveFormat(44100, 16, StereoAudio ? 2 : 1);
        }

        public void Start(int Delay)
        {
            new Thread(new ParameterizedThreadStart((e) =>
                {
                    Thread.Sleep((int)e);

                    if (!IsGif) InitSharpAvi();
                    else InitGif();

                })).Start(Delay);
        }

        void InitGif()
        {
            GifWriter = CreateGifWriter();

            InitAudio();

            screenThread = new Thread(RecordScreenAsGif)
            {
                Name = typeof(GifWriter).Name + ".RecordScreen",
                IsBackground = true
            };

            screenThread.Start();

            if (audioSource != null)
            {
                WaveWriter = CreateWaveWriter();

                audioSource.DataAvailable += AudioDataAvailable;

                audioSource.StartRecording();
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
                    audioStream = CreateAudioStream(writer);
                    audioStream.Name = "Loopback";
                }

                audioSource = new WasapiLoopbackCapture(dev) { ShareMode = AudioClientShareMode.Shared };
            }
            else
            {
                int Id = int.Parse(AudioSourceId);
                
                if (Id != -1)
                {
                    if (CaptureVideo)
                    {
                        audioStream = CreateAudioStream(writer);
                        audioStream.Name = "Voice";
                    }

                    audioSource = new WaveInEvent
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
                writer = CreateAviWriter();

                // Create video stream
                videoStream = CreateVideoStream(writer);
                // Set only name. Other properties were when creating stream, 
                // either explicitly by arguments or implicitly by the encoder used
                videoStream.Name = "Captura";
            }

            InitAudio();

            if (CaptureVideo)
            {
                screenThread = new Thread(RecordScreen)
                {
                    Name = typeof(Recorder).Name + ".RecordScreen",
                    IsBackground = true
                };
            }
            else WaveWriter = CreateWaveWriter();

            if (CaptureVideo) screenThread.Start();

            if (audioSource != null)
            {
                audioSource.DataAvailable += AudioDataAvailable;

                if (CaptureVideo)
                {
                    videoFrameWritten.Set();
                    audioBlockWritten.Reset();
                }

                audioSource.StartRecording();
            }
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
                if (!stopThread.SafeWaitHandle.IsClosed
                    && !stopThread.SafeWaitHandle.IsInvalid)
                    stopThread.Set();
                screenThread.Join(500);
                if (screenThread.IsAlive) screenThread.Abort();
            }

            if (audioSource != null)
            {
                audioSource.StopRecording();
                audioSource.DataAvailable -= AudioDataAvailable;
            }

            if (CaptureVideo)
            {
                // Close writer: the remaining data is written to a file and file is closed
                writer.Close();

                if (!stopThread.SafeWaitHandle.IsClosed) stopThread.Close();
            }
            else if (IsGif)
            {
                GifWriter.Dispose();
                if (!stopThread.SafeWaitHandle.IsClosed) stopThread.Close();
                if (WaveWriter != null) WaveWriter.Dispose();
            }
            else WaveWriter.Dispose();
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                if (SilencePlayer != null) SilencePlayer.Pause();

                if (CaptureVideo || IsGif) screenThread.Suspend();

                if (audioSource != null) audioSource.StopRecording();

                IsPaused = true;
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                if (SilencePlayer != null) SilencePlayer.Play();

                if (CaptureVideo || IsGif) screenThread.Resume();

                if (audioSource != null)
                {
                    if (CaptureVideo)
                    {
                        videoFrameWritten.Set();
                        audioBlockWritten.Reset();
                    }
                    audioSource.StartRecording();
                }

                IsPaused = false;
            }
        }

        void RecordScreen()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
                var buffer = new byte[Commons.DesktopWidth * Commons.DesktopHeight * 4];
                Task videoWriteTask = null;
                var isFirstFrame = true;
                var timeTillNextFrame = TimeSpan.Zero;

                while (!stopThread.WaitOne(timeTillNextFrame))
                {
                    var timestamp = DateTime.Now;

                    ScreenShot(buffer);

                    // Wait for the previous frame is written
                    if (!isFirstFrame)
                    {
                        videoWriteTask.Wait();
                        videoFrameWritten.Set();
                    }

                    if (audioStream != null && !IsLoopback)
                        if (WaitHandle.WaitAny(new WaitHandle[] { audioBlockWritten, stopThread }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero) timeTillNextFrame = TimeSpan.Zero;

                    isFirstFrame = false;
                }

                // Wait for the last frame is written
                if (!isFirstFrame) videoWriteTask.Wait();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        void RecordScreenAsGif()
        {
            try
            {
                var frameInterval = TimeSpan.FromMilliseconds(GifWriter.DefaultFrameDelay);
                var timeTillNextFrame = TimeSpan.Zero;
                Task GifWriteTask = null;
                var isFirstFrame = true;
                Image ScreenshotImage;

                while (!stopThread.WaitOne(timeTillNextFrame))
                {
                    var timestamp = DateTime.Now;

                    ScreenshotImage = ScreenShot(hWnd(), IncludeCursor(), true, BgColor, CaptureMouseClicks, CaptureKeyStrokes);

                    if (!isFirstFrame) GifWriteTask.Wait();

                    if (audioStream != null)
                        if (WaitHandle.WaitAny(new WaitHandle[] { audioBlockWritten, stopThread }) == 1)
                            break;

                    // Start asynchronous (encoding and) writing of the new frame
                    GifWriteTask = GifWriter.WriteFrameAsync(ScreenshotImage);

                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero) timeTillNextFrame = TimeSpan.Zero;

                    isFirstFrame = false;
                }

                // Wait for the last frame is written
                if (!isFirstFrame) GifWriteTask.Wait();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public static Bitmap ScreenShot(IntPtr hWnd, bool IncludeCursor, bool ScreenCasting, Color BgColor,
            bool CaptureMouseClicks = false, bool CaptureKeyStrokes = false)
        {
            int CursorX = 0, CursorY = 0;
            Rectangle Rect = default(Rectangle);

            if (hWnd != Commons.DesktopHandle)
            {
                var rect = new Rectangle();
                User32.GetWindowRect(hWnd, ref rect);

                Rect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                if (!ScreenCasting) User32.SetWindowPos(hWnd, (IntPtr)(-1), 0, 0, 0, 0, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoSize);
            }
            else Rect = Commons.DesktopRectangle;

            var BMP = new Bitmap(Commons.DesktopWidth, Commons.DesktopHeight);
            using (var g = Graphics.FromImage(BMP))
            {
                if (BgColor != Color.Transparent) g.FillRectangle(new SolidBrush(BgColor), Commons.DesktopRectangle);

                g.CopyFromScreen(Rect.Location, Rect.Location, Rect.Size, CopyPixelOperation.SourceCopy);

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
                if (ScreenCasting && CaptureMouseClicks && Commons.MouseClicked)
                {
                    var curPos = User32.CursorPosition;
                    g.DrawArc(new Pen(Color.Black, 1), curPos.X - 40, curPos.Y - 40, 80, 80, 0, 360);

                    Commons.MouseClicked = false;
                }
                #endregion

                #region KeyStrokes
                if (ScreenCasting && CaptureKeyStrokes
                    && Commons.LastKeyPressed != System.Windows.Forms.Keys.None)
                {
                    g.DrawString(Commons.LastKeyPressed.ToString(),
                        new Font(FontFamily.GenericMonospace, 100),
                        new SolidBrush(Color.Black), 100, 100);

                    Commons.LastKeyPressed = System.Windows.Forms.Keys.None;
                }
                #endregion

                g.Flush();
            }

            if (!ScreenCasting) User32.SetWindowPos(hWnd, (IntPtr)(-2), 0, 0, 0, 0, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoSize);

            return BMP;
        }

        public void ScreenShot(byte[] Buffer)
        {
            using (var BMP = ScreenShot(hWnd(), IncludeCursor(), true, BgColor, CaptureMouseClicks, CaptureKeyStrokes))
            {
                var bits = BMP.LockBits(Commons.DesktopRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                BMP.UnlockBits(bits);
            }
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e)
        {
            if (CaptureVideo)
            {
                if (IsLoopback) audioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);
                else
                {
                    var signalled = WaitHandle.WaitAny(new WaitHandle[] { videoFrameWritten, stopThread });
                    if (signalled == 0)
                    {
                        audioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);

                        audioBlockWritten.Set();
                    }
                }
            }
            else WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }
}
