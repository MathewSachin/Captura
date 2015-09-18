using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Fluent;
using ManagedWin32;
using NWaveIn;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Captura
{
    class RecorderParams
    {
        public RecorderParams(string filename, Spinner FrameRate, FourCC Encoder, Spinner Quality,
            int AudioSourceId, SupportedWaveFormat wf, ToggleButton EncodeAudio, System.Windows.Controls.Slider AudioQuality, ToggleButton IncludeCursor)
        {
            FileName = filename;
            FramesPerSecond = (int)FrameRate.Value;
            Codec = Encoder;
            this.Quality = (int)Quality.Value;
            this.AudioSourceId = AudioSourceId;
            this.EncodeAudio = EncodeAudio.IsChecked.Value;
            AudioBitRate = Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt((int)AudioQuality.Value);
            this.IncludeCursor = IncludeCursor.IsChecked.Value;

            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            ScreenHeight = (int)Math.Round(SystemParameters.PrimaryScreenHeight * toDevice.M22);
            ScreenWidth = (int)Math.Round(SystemParameters.PrimaryScreenWidth * toDevice.M11);

            switch (wf)
            {
                case SupportedWaveFormat.WAVE_FORMAT_44M16:
                    WaveFormat = new WaveFormat(44100, 16, 1);
                    break;
                case SupportedWaveFormat.WAVE_FORMAT_44S16:
                    WaveFormat = new WaveFormat(44100, 16, 2);
                    break;
            }
        }

        string FileName;
        public int FramesPerSecond, Quality, AudioSourceId, AudioBitRate;
        FourCC Codec;
        public bool EncodeAudio, IncludeCursor;

        public int ScreenHeight { get; private set; }
        public int ScreenWidth { get; private set; }

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
            if (Codec == KnownFourCCs.Codecs.Uncompressed) return writer.AddUncompressedVideoStream(ScreenWidth, ScreenHeight);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg) return writer.AddMotionJpegVideoStream(ScreenWidth, ScreenHeight, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(ScreenWidth, ScreenHeight, (double)writer.FramesPerSecond,
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

        void RecordScreen()
        {
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            var buffer = new byte[Params.ScreenWidth * Params.ScreenHeight * 4];
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

        void Screenshot(byte[] Buffer)
        {
            using (var bitmap = new Bitmap(Params.ScreenWidth, Params.ScreenHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                try { graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(Params.ScreenWidth, Params.ScreenHeight)); }
                catch { }

                if (Params.IncludeCursor)
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

                var bits = bitmap.LockBits(new Rectangle(0, 0, Params.ScreenWidth, Params.ScreenHeight),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                bitmap.UnlockBits(bits);
            }
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
