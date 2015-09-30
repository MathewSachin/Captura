using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using ManagedWin32.Api;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using ManagedWin32;

namespace Captura
{
    class RecorderParams
    {
        public IntPtr hWnd;

        public static readonly IntPtr Desktop = WindowHandler.DesktopWindow.Handle;

        public RecorderParams(string filename, int FrameRate, FourCC Encoder, int Quality,
            int AudioSourceId, bool UseStereo, bool EncodeAudio, int AudioQuality, bool IncludeCursor, IntPtr hWnd)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;
            this.AudioSourceId = AudioSourceId;
            this.EncodeAudio = EncodeAudio;
            AudioBitRate = Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt(AudioQuality);
            this.IncludeCursor = IncludeCursor;
            this.hWnd = hWnd;

            if (hWnd == IntPtr.Zero)
            {
                System.Windows.Media.Matrix toDevice;
                using (var source = new HwndSource(new HwndSourceParameters()))
                    toDevice = source.CompositionTarget.TransformToDevice;

                Height = (int)Math.Round(SystemParameters.PrimaryScreenHeight * toDevice.M22);
                Width = (int)Math.Round(SystemParameters.PrimaryScreenWidth * toDevice.M11);
            }
            else
            {
                var rect = new RECT();
                User32.GetWindowRect(hWnd, ref rect);

                Width = rect.Right - rect.Left;
                Height = rect.Bottom - rect.Top;
            }

            WaveFormat = new WaveFormat(44100, 16, UseStereo ? 2 : 1);
        }

        string FileName;
        public int FramesPerSecond, Quality, AudioSourceId, AudioBitRate;
        FourCC Codec;
        public bool EncodeAudio, IncludeCursor;

        public int Height;
        public int Width;

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed) return writer.AddUncompressedVideoStream(Width, Height);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg) return writer.AddMotionJpegVideoStream(Width, Height, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(Width, Height, (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }

        public IAviAudioStream CreateAudioStream(AviWriter writer)
        {
            // Create encoding or simple stream based on settings
            if (EncodeAudio)
            {
                // LAME DLL path is set in App.OnStartup()
                return writer.AddMp3AudioStream(WaveFormat.Channels, WaveFormat.SampleRate, AudioBitRate);
            }
            else return writer.AddAudioStream(WaveFormat.Channels, WaveFormat.SampleRate, WaveFormat.BitsPerSample);
        }

        public WaveFormat WaveFormat { get; private set; }
    }

    class Recorder : IDisposable
    {
        #region Fields
        AviWriter writer;
        RecorderParams Params;
        IAviVideoStream videoStream;
        IAviAudioStream audioStream;
        WaveInEvent audioSource;
        Thread screenThread;
        ManualResetEvent stopThread = new ManualResetEvent(false);
        AutoResetEvent videoFrameWritten = new AutoResetEvent(false),
            audioBlockWritten = new AutoResetEvent(false);
        public bool IsPaused = false;
        #endregion

        public Recorder(RecorderParams Params)
        {
            this.Params = Params;

            // Create AVI writer and specify FPS
            writer = Params.CreateAviWriter();

            // Create video stream
            videoStream = Params.CreateVideoStream(writer);
            // Set only name. Other properties were when creating stream, 
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Captura";

            if (Params.AudioSourceId != -1)
            {
                try
                {
                    var waveFormat = Params.WaveFormat;

                    audioStream = Params.CreateAudioStream(writer);
                    audioStream.Name = "Voice";

                    audioSource = new WaveInEvent
                    {
                        DeviceNumber = Params.AudioSourceId,
                        WaveFormat = waveFormat,
                        // Buffer size to store duration of 1 frame
                        BufferMilliseconds = (int)Math.Ceiling(1000 / writer.FramesPerSecond),
                        NumberOfBuffers = 3,
                    };
                }
                catch
                {
                    //var dev = new MMDeviceEnumerator().GetDevice(audioSourceId);

                    //if (dev.DataFlow == DataFlow.All || dev.DataFlow == DataFlow.Render)
                    //{
                    //    var waveFormat = dev.AudioClient.MixFormat;

                    //    audioStream = CreateAudioStream(waveFormat, encodeAudio, audioBitRate);
                    //    audioStream.Name = "Loopback";

                    //    audioSource = new WasapiLoopbackCapture(dev) { ShareMode = AudioClientShareMode.Shared };
                    //}
                }

                audioSource.DataAvailable += AudioDataAvailable;
            }

            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };

            if (audioSource != null)
            {
                videoFrameWritten.Set();
                audioBlockWritten.Reset();
                audioSource.StartRecording();
            }
            screenThread.Start();
        }

        public void Dispose()
        {
            if (IsPaused) Resume();

            stopThread.Set();
            screenThread.Join();

            if (audioSource != null)
            {
                audioSource.StopRecording();
                audioSource.DataAvailable -= AudioDataAvailable;
            }

            // Close writer: the remaining data is written to a file and file is closed
            writer.Close();

            stopThread.Close();
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                screenThread.Suspend();

                if (audioSource != null) audioSource.StopRecording();

                IsPaused = true;
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                screenThread.Resume();

                if (audioSource != null)
                {
                    videoFrameWritten.Set();
                    audioBlockWritten.Reset();
                    audioSource.StartRecording();
                }

                IsPaused = false;
            }
        }

        void RecordScreen()
        {
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            var buffer = new byte[Params.Width * Params.Height * 4];
            Task videoWriteTask = null;
            var isFirstFrame = true;
            var timeTillNextFrame = TimeSpan.Zero;
            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                Screenshot(buffer);

                // Wait for the previous frame is written
                if (!isFirstFrame)
                {
                    videoWriteTask.Wait();
                    videoFrameWritten.Set();
                }

                if (audioStream != null)
                {
                    var signalled = WaitHandle.WaitAny(new WaitHandle[] { audioBlockWritten, stopThread });
                    if (signalled == 1) break;
                }

                // Start asynchronous (encoding and) writing of the new frame
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero) timeTillNextFrame = TimeSpan.Zero;

                isFirstFrame = false;
            }

            // Wait for the last frame is written
            if (!isFirstFrame) videoWriteTask.Wait();
        }

        // CaptureBlt ??
        public void Screenshot(byte[] Buffer)
        {
            IntPtr hDC = User32.GetWindowDC(Params.hWnd),
                hMemDC = Gdi32.CreateCompatibleDC(hDC),
                hIcon = IntPtr.Zero;

            int CursorX = 0, CursorY = 0;

            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hDC, Params.Width, Params.Height);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = Gdi32.SelectObject(hMemDC, hBitmap);

                if (Params.hWnd != RecorderParams.Desktop)
                {
                    var rect = new RECT();
                    User32.GetWindowRect(Params.hWnd, ref rect);

                    Params.Width = rect.Right - rect.Left;
                    Params.Height = rect.Bottom - rect.Top;
                }

                Gdi32.BitBlt(hMemDC, 0, 0, Params.Width, Params.Height, hDC, 0, 0, PatBltTypes.SRCCOPY);

                Gdi32.SelectObject(hMemDC, hOld);
            }
            
            #region Include Cursor
            if (Params.hWnd == IntPtr.Zero && Params.IncludeCursor)
            {
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
            }
            #endregion
            
            using (var BMP = new Bitmap(Params.Width, Params.Height))
            {
                using (var g = Graphics.FromImage(BMP))
                {
                    g.DrawImage(Bitmap.FromHbitmap(hBitmap), 0, 0);

                    if (hIcon != IntPtr.Zero)
                    {
                        Bitmap CursorBMP = Icon.FromHandle(hIcon).ToBitmap();
                        g.DrawImage(CursorBMP, CursorX, CursorY, CursorBMP.Width, CursorBMP.Height);
                    }

                    g.Flush();

                    var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    BMP.UnlockBits(bits);
                }
            }

            Gdi32.DeleteObject(hBitmap);
            Gdi32.DeleteDC(hMemDC);
            User32.ReleaseDC(Params.hWnd, hDC);
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e)
        {
            var signalled = WaitHandle.WaitAny(new WaitHandle[] { videoFrameWritten, stopThread });
            if (signalled == 0)
            {
                audioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);
                audioBlockWritten.Set();
            }
        }
    }
}
