﻿using Screna.Audio;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SharpAvi.Codecs;
using SharpAvi.Output;
using AviInternalWriter = SharpAvi.Output.AviWriter;
using Screna;

namespace Captura.Models
{
    /// <summary>
    /// Writes an AVI file.
    /// </summary>
    public class AviWriter : IVideoFileWriter
    {
        #region Fields
        readonly AviInternalWriter _writer;
        IAviVideoStream _videoStream;
        IAviAudioStream _audioStream;
        readonly byte[] _videoBuffer;
        readonly AviCodec _codec;
        
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

        int lastFrameHash;
        
        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        public void WriteFrame(Bitmap Image)
        {
            var hash = Image.GetHashCode();

            if (lastFrameHash != hash)
            {
                using (Image)
                {
                    var bits = Image.LockBits(new Rectangle(Point.Empty, Image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
                    Image.UnlockBits(bits);
                }

                lastFrameHash = hash;
            }

            _videoStream.WriteFrame(true, _videoBuffer, 0, _videoBuffer.Length);
        }

        void CreateVideoStream(int Width, int Height)
        {
            // Select encoder type based on FOURCC of codec
            if (_codec == AviCodec.Uncompressed)
                _videoStream = _writer.AddUncompressedVideoStream(Width, Height);
            else if (_codec == AviCodec.MotionJpeg)
                _videoStream = _writer.AddMotionJpegVideoStream(Width, Height, _codec.Quality);
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

            _videoStream.Name = "ScrenaVideo";
        }

        void CreateAudioStream(IAudioProvider AudioProvider)
        {
            _audioStream = _writer.AddEncodingAudioStream(new IAudioProviderAdapter(AudioProvider));

            _audioStream.Name = "ScrenaAudio";
        }

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
        public void WriteAudio(byte[] Buffer, int Length)
        {
            _audioStream?.WriteBlock(Buffer, 0, Length);
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _writer?.Close();
        }
    }
}
