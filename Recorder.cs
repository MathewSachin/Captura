using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using NWaveIn;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using ManagedWin32;

namespace Captura
{
    class Recorder : IDisposable
    {
        int screenWidth, screenHeight;
        AviWriter writer;
        IAviVideoStream videoStream;
        IAviAudioStream audioStream;
        WaveInEvent audioSource;
        Thread screenThread;
        bool IncludeCursor;
        ManualResetEvent stopThread = new ManualResetEvent(false);
        AutoResetEvent videoFrameWritten = new AutoResetEvent(false),
            audioBlockWritten = new AutoResetEvent(false);

        public Recorder(string fileName, int fps, FourCC codec, int quality, int audioSourceId,
            SupportedWaveFormat audioWaveFormat, bool encodeAudio, int audioBitRate, bool IncludeCursor)
        {
            this.IncludeCursor = IncludeCursor;

            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            screenWidth = (int)Math.Round(SystemParameters.PrimaryScreenWidth * toDevice.M11);
            screenHeight = (int)Math.Round(SystemParameters.PrimaryScreenHeight * toDevice.M22);

            // Create AVI writer and specify FPS
            writer = new AviWriter(fileName)
            {
                FramesPerSecond = fps,
                EmitIndex1 = true,
            };

            // Create video stream
            videoStream = CreateVideoStream(codec, quality);
            // Set only name. Other properties were when creating stream, 
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Captura";

            if (audioSourceId != -1)
            {
                try
                {
                    var waveFormat = ToWaveFormat(audioWaveFormat);

                    audioStream = CreateAudioStream(waveFormat, encodeAudio, audioBitRate);
                    audioStream.Name = "Voice";

                    audioSource = new WaveInEvent
                    {
                        DeviceNumber = audioSourceId,
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

                audioSource.DataAvailable += audioSource_DataAvailable;
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

        IAviVideoStream CreateVideoStream(FourCC codec, int quality)
        {
            // Select encoder type based on FOURCC of codec
            if (codec == KnownFourCCs.Codecs.Uncompressed) return writer.AddUncompressedVideoStream(screenWidth, screenHeight);
            else if (codec == KnownFourCCs.Codecs.MotionJpeg) return writer.AddMotionJpegVideoStream(screenWidth, screenHeight, quality);
            else
            {
                return writer.AddMpeg4VideoStream(screenWidth, screenHeight, (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: quality,
                    codec: codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }

        IAviAudioStream CreateAudioStream(WaveFormat waveFormat, bool encode, int bitRate)
        {
            // Create encoding or simple stream based on settings
            if (encode)
            {
                // LAME DLL path is set in App.OnStartup()
                return writer.AddMp3AudioStream(waveFormat.Channels, waveFormat.SampleRate, bitRate);
            }
            else return writer.AddAudioStream(waveFormat.Channels, waveFormat.SampleRate, waveFormat.BitsPerSample);
        }

        static WaveFormat ToWaveFormat(SupportedWaveFormat waveFormat)
        {
            switch (waveFormat)
            {
                case SupportedWaveFormat.WAVE_FORMAT_44M16:
                    return new WaveFormat(44100, 16, 1);
                case SupportedWaveFormat.WAVE_FORMAT_44S16:
                    return new WaveFormat(44100, 16, 2);
                default:
                    throw new NotSupportedException("Wave formats other than '16-bit 44.1kHz' are not currently supported.");
            }
        }

        public void Dispose()
        {
            stopThread.Set();
            screenThread.Join();
            if (audioSource != null)
            {
                audioSource.StopRecording();
                audioSource.DataAvailable -= audioSource_DataAvailable;
            }

            // Close writer: the remaining data is written to a file and file is closed
            writer.Close();

            stopThread.Close();
        }

        void RecordScreen()
        {
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            var buffer = new byte[screenWidth * screenHeight * 4];
            Task videoWriteTask = null;
            var isFirstFrame = true;
            var timeTillNextFrame = TimeSpan.Zero;
            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                GetScreenshot(buffer);

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

        void GetScreenshot(byte[] buffer)
        {
            using (var bitmap = new Bitmap(screenWidth, screenHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                try { graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight)); }
                catch { }

                if (IncludeCursor)
                {
                    int cursorX = 0, cursorY = 0;
                    Bitmap cursorBMP;

                    cursorBMP = ScreenCapture.CaptureCursor(ref cursorX, ref cursorY);
                                        
                    if (cursorBMP != null)
                    {
                        Rectangle r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);

                        graphics.DrawImage(cursorBMP, r);
                        graphics.Flush();
                    }
                }

                var bits = bitmap.LockBits(new Rectangle(0, 0, screenWidth, screenHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                bitmap.UnlockBits(bits);
            }
        }
        
        void audioSource_DataAvailable(object sender, WaveInEventArgs e)
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
