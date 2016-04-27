using Screna.Audio;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharpAvi.Codecs;
using SharpAvi.Output;
using AviInternalWriter = SharpAvi.Output.AviWriter;
using IAudioEncoder = Screna.Audio.IAudioEncoder;

namespace Screna.Avi
{
    /// <summary>
    /// Writes an AVI file.
    /// </summary>
    public class AviWriter : IVideoFileWriter
    {
        #region Fields
        AviInternalWriter _writer;
        IAviVideoStream _videoStream;
        IAviAudioStream _audioStream;
        IAudioProvider _audioFacade;
        byte[] _videoBuffer;
        readonly string _fileName;
        readonly IAudioEncoder _audioEncoder;
        readonly AviCodec _codec;

        /// <summary>
        /// Video Frame Rate.
        /// </summary>
        public int FrameRate { get; private set; }
        
        /// <summary>
        /// Gets whether Audio is recorded.
        /// </summary>
        public bool SupportsAudio => _audioStream != null;
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AviWriter"/>.
        /// </summary>
        /// <param name="FileName">Output file path.</param>
        /// <param name="Codec">The Avi Codec.</param>
        /// <param name="AudioEncoder">The <see cref="IAudioEncoder"/> to use to encode Audio... null (default) = don't encode audio.</param>
        public AviWriter(string FileName,
                         AviCodec Codec,
                         IAudioEncoder AudioEncoder = null)
        {
            _fileName = FileName;
            _audioEncoder = AudioEncoder;
            _codec = Codec;
        }

        /// <summary>
        /// Initialises the <see cref="IVideoFileWriter"/>. Usually called by an <see cref="IRecorder"/>.
        /// </summary>
        /// <param name="ImageProvider">The Image Provider.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        /// <param name="AudioProvider">The Audio Provider.</param>
        public void Init(IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider)
        {
            this.FrameRate = FrameRate;
            _audioFacade = AudioProvider;
            _videoBuffer = new byte[ImageProvider.Width * ImageProvider.Height * 4];

            _writer = new AviInternalWriter(_fileName)
            {
                FramesPerSecond = FrameRate,
                EmitIndex1 = true
            };

            CreateVideoStream(ImageProvider.Width, ImageProvider.Height, _codec);

            if (AudioProvider != null)
                CreateAudioStream(_audioFacade, _audioEncoder);
        }

        /// <summary>
        /// Asynchronously writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        /// <returns>The Task Object.</returns>
        public Task WriteFrameAsync(Bitmap Image)
        {
            var bits = Image.LockBits(new Rectangle(Point.Empty, Image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
            Image.UnlockBits(bits);

            return _videoStream.WriteFrameAsync(true, _videoBuffer, 0, _videoBuffer.Length);
        }

        #region Private Methods
        void CreateVideoStream(int Width, int Height, AviCodec Codec)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == AviCodec.Uncompressed)
                _videoStream = _writer.AddUncompressedVideoStream(Width, Height);
            else if (Codec == AviCodec.MotionJpeg)
                _videoStream = _writer.AddMotionJpegVideoStream(Width, Height, Codec.Quality);
            else
            {
                _videoStream = _writer.AddMpeg4VideoStream(Width, Height,
                    (double)_writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    0,
                    Codec.Quality,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    Codec.FourCC,
                    true);
            }

            _videoStream.Name = "ScrenaVideo";
        }

        void CreateAudioStream(IAudioProvider AudioFacade, IAudioEncoder AudioEncoder)
        {
            // Create encoding or simple stream based on settings
            _audioStream = AudioEncoder != null 
                ? _writer.AddEncodingAudioStream(new IAudioEncoderWrapper(AudioEncoder))
                : _writer.AddEncodingAudioStream(new IAudioProviderWrapper(AudioFacade));

            _audioStream.Name = "ScrenaAudio";
        }
        #endregion

        /// <summary>
        /// Enumerates all available Avi Encoders.
        /// </summary>
        public static IEnumerable<AviCodec> EnumerateEncoders()
        {
            yield return AviCodec.Uncompressed;
            yield return AviCodec.MotionJpeg;
            foreach (var codec in Mpeg4VideoEncoderVcm.GetAvailableCodecs())
                yield return new AviCodec(codec.Codec, codec.Name);
        }

        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length) => _audioStream?.WriteBlock(Buffer, 0, Length);

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _writer?.Close();
            
            _audioFacade?.Dispose();
        }
    }
}
