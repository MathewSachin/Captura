using System;
using Captura.Audio;
using Captura.Video;
using SharpAvi.Codecs;
using SharpAvi.Output;
using AviInternalWriter = SharpAvi.Output.AviWriter;

namespace Captura.SharpAvi
{
    /// <summary>
    /// Writes an AVI file.
    /// </summary>
    class AviWriter : IVideoFileWriter
    {
        #region Fields
        AviInternalWriter _writer;
        IAviVideoStream _videoStream;
        IAviAudioStream _audioStream;
        byte[] _videoBuffer;
        readonly AviCodec _codec;
        readonly object _syncLock = new object();
        
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
        /// <param name="ImageProvider">The image source.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        /// <param name="AudioProvider">The audio source. null = no audio.</param>
        public AviWriter(string FileName, AviCodec Codec, IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider = null)
        {
            _codec = Codec;

            _videoBuffer = new byte[ImageProvider.Width * ImageProvider.Height * 4];

            _writer = new AviInternalWriter(FileName)
            {
                FramesPerSecond = FrameRate,
                EmitIndex1 = true
            };

            CreateVideoStream(ImageProvider.Width, ImageProvider.Height);

            if (AudioProvider != null)
                CreateAudioStream(AudioProvider);
        }
        
        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        public void WriteFrame(IBitmapFrame Frame)
        {
            if (!(Frame is RepeatFrame))
            {
                using (Frame)
                {
                    Frame.CopyTo(_videoBuffer);
                }
            }

            lock (_syncLock)
                _videoStream.WriteFrame(true, _videoBuffer, 0, _videoBuffer.Length);
        }

        void CreateVideoStream(int Width, int Height)
        {
            // Select encoder type based on FOURCC of codec
            if (_codec == AviCodec.Uncompressed)
                _videoStream = _writer.AddUncompressedVideoStream(Width, Height);
            else if (_codec == AviCodec.MotionJpeg)
            {
                // MotionJpegVideoStream implementation allocates multiple WriteableBitmap for every thread
                // Use SingleThreadWrapper to reduce allocation
                var encoderFactory = new Func<IVideoEncoder>(() => new MotionJpegVideoEncoderWpf(Width, Height, _codec.Quality));
                var encoder = new SingleThreadedVideoEncoderWrapper(encoderFactory);

                _videoStream = _writer.AddEncodingVideoStream(encoder, true, Width, Height);
            }
            else
            {
                _videoStream = _writer.AddMpeg4VideoStream(Width, Height,
                    (double)_writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    0,
                    _codec.Quality,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    _codec.FourCC,
                    true);
            }

            _videoStream.Name = "Video";
        }

        void CreateAudioStream(IAudioProvider AudioProvider)
        {
            _audioStream = _writer.AddEncodingAudioStream(new IAudioProviderAdapter(AudioProvider));

            _audioStream.Name = "Audio";
        }

        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Offset, int Length)
        {
            lock (_syncLock)
                _audioStream?.WriteBlock(Buffer, Offset, Length);
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                _writer.Close();
                _writer = null;

                _videoStream = null;
                _audioStream = null;
            }

            _videoBuffer = null;
        }
    }
}
