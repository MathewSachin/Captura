using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using ManagedWin32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Captura
{
    class RecorderParams
    {
        public int StartDelay;
        public MainWindow MainWindow;

        public static readonly int DesktopHeight, DesktopWidth;

        public static readonly IntPtr Desktop = WindowHandler.DesktopWindow.Handle;

        static RecorderParams()
        {
            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            DesktopHeight = (int)Math.Round(SystemParameters.PrimaryScreenHeight * toDevice.M22);
            DesktopWidth = (int)Math.Round(SystemParameters.PrimaryScreenWidth * toDevice.M11);
        }

        public RecorderParams(MainWindow MainWindow, string filename, int FrameRate, FourCC Encoder, int Quality,
            string AudioSourceId, bool UseStereo, bool EncodeAudio, int AudioQuality, int StartDelay)
        {
            this.MainWindow = MainWindow;

            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;
            this.AudioSourceId = AudioSourceId;
            this.EncodeAudio = EncodeAudio;
            AudioBitRate = Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt(AudioQuality);
            CaptureVideo = hWnd.ToInt32() != -1;
            this.StartDelay = StartDelay;

            int val;
            IsLoopback = !int.TryParse(AudioSourceId, out val);

            WaveFormat = IsLoopback ? LoopbackDevice.AudioClient.MixFormat : new WaveFormat(44100, 16, UseStereo ? 2 : 1);
        }

        public bool IncludeCursor
        {
            get
            {
                return (bool)MainWindow.Dispatcher.Invoke(new Func<bool>(() => MainWindow.IncludeCursor.IsChecked.Value));
            }
        }

        public IntPtr hWnd
        {
            get
            {
                return (IntPtr)MainWindow.Dispatcher.Invoke(new Func<IntPtr>(() => MainWindow.SelectedWindow));
            }
        }

        public MMDevice LoopbackDevice { get { return new MMDeviceEnumerator().GetDevice(AudioSourceId); } }

        public string FileName, AudioSourceId;
        public int FramesPerSecond, Quality, AudioBitRate;
        FourCC Codec;
        public bool EncodeAudio, CaptureVideo;

        public bool IsLoopback;

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
            if (Codec == KnownFourCCs.Codecs.Uncompressed) return writer.AddUncompressedVideoStream(DesktopWidth, DesktopHeight);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg) return writer.AddMotionJpegVideoStream(DesktopWidth, DesktopHeight, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(DesktopWidth, DesktopHeight, (double)writer.FramesPerSecond,
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
            if (IsLoopback) return writer.AddAudioStream(WaveFormat.Channels, WaveFormat.SampleRate, WaveFormat.BitsPerSample, AudioFormats.Float);
            else if (EncodeAudio)
            {
                // LAME DLL path is set in App.OnStartup()
                return writer.AddMp3AudioStream(WaveFormat.Channels, WaveFormat.SampleRate, AudioBitRate);
            }
            else return writer.AddAudioStream(WaveFormat.Channels, WaveFormat.SampleRate, WaveFormat.BitsPerSample);
        }

        public WaveFileWriter CreateWaveWriter() { return new WaveFileWriter(FileName, WaveFormat); }

        public WaveFormat WaveFormat { get; private set; }
    }
}