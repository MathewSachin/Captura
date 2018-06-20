using System;
using System.IO;

namespace Screna
{
    /// <summary>
    /// Creates a GIF using .Net GIF encoding and additional animation headers.
    /// </summary>
    public class GifWriter : IVideoFileWriter
    {
        #region Fields
        const long SourceGlobalColorInfoPosition = 10,
            SourceImageBlockPosition = 789;

        readonly BinaryWriter _writer;
        bool _firstFrame = true;
        readonly int _defaultFrameDelay, _repeat;
        #endregion

        /// <summary>
        /// Creates a new instance of GifWriter.
        /// </summary>
        /// <param name="OutStream">The <see cref="Stream"/> to output the Gif to.</param>
        /// <param name="FrameRate">Fame Rate.</param>
        /// <param name="Repeat">No of times the Gif should repeat... -1 to repeat indefinitely.</param>
        public GifWriter(Stream OutStream, int FrameRate, int Repeat = -1)
        {
            if (Repeat < -1)
                throw new ArgumentOutOfRangeException(nameof(Repeat));

            _writer = new BinaryWriter(OutStream ?? throw new ArgumentNullException(nameof(OutStream)));
            _defaultFrameDelay = 1000 / FrameRate;
            _repeat = Repeat;
        }

        /// <summary>
        /// Creates a new instance of GifWriter.
        /// </summary>
        /// <param name="FileName">The path to the file to output the Gif to.</param>
        /// <param name="FrameRate">Frame Rate.</param>
        /// <param name="Repeat">No of times the Gif should repeat... -1 to repeat indefinitely.</param>
        public GifWriter(string FileName, int FrameRate, int Repeat = -1)
            : this(new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read), FrameRate, Repeat) { }

        /// <summary>
        /// <see cref="GifWriter"/> does not Support Audio.
        /// </summary>
        public void WriteAudio(byte[] Buffer, int Count) { }

        readonly MemoryStream _gifStream = new MemoryStream();

        int _width, _height;

        /// <summary>
        /// Adds a frame to this animation.
        /// </summary>
        public void WriteFrame(IBitmapFrame Frame, int Delay)
        {
            _gifStream.Position = 0;
            
            if (_firstFrame)
            {
                if (Frame is RepeatFrame)
                    return;

                _width = Frame.Width;
                _height = Frame.Height;
            }

            if (!(Frame is RepeatFrame))
            {
                using (Frame)
                    Frame.SaveGif(_gifStream);
            }

            // Steal the global color table info
            if (_firstFrame)
                InitHeader(_gifStream, _writer, _width, _height);

            WriteGraphicControlBlock(_gifStream, _writer, Delay);
            WriteImageBlock(_gifStream, _writer, !_firstFrame, 0, 0, _width, _height);
            
            if (_firstFrame)
                _firstFrame = false;
        }

        /// <summary>
        /// Writes a Image frame.
        /// </summary>
        /// <param name="Image">Image frame to write.</param>
        public void WriteFrame(IBitmapFrame Image) => WriteFrame(Image, _defaultFrameDelay);
        
        /// <summary>
        /// <see cref="GifWriter"/> does not support Audio.
        /// </summary>
        public bool SupportsAudio => false;

        #region Write
        void InitHeader(Stream SourceGif, BinaryWriter Writer, int Width, int Height)
        {
            // File Header
            Writer.Write("GIF".ToCharArray()); // File type
            Writer.Write("89a".ToCharArray()); // File Version

            Writer.Write((short)Width); // Initial Logical Width
            Writer.Write((short)Height); // Initial Logical Height

            SourceGif.Position = SourceGlobalColorInfoPosition;
            Writer.Write((byte)SourceGif.ReadByte()); // Global Color Table Info
            Writer.Write((byte)0); // Background Color Index
            Writer.Write((byte)0); // Pixel aspect ratio
            WriteColorTable(SourceGif, Writer);

            // App Extension Header for Repeating
            if (_repeat == -1)
                return;

            Writer.Write(unchecked((short)0xff21)); // Application Extension Block Identifier
            Writer.Write((byte)0x0b); // Application Block Size
            Writer.Write("NETSCAPE2.0".ToCharArray()); // Application Identifier
            Writer.Write((byte)3); // Application block length
            Writer.Write((byte)1);
            Writer.Write((short)_repeat); // Repeat count for images.
            Writer.Write((byte)0); // terminator
        }

        static void WriteColorTable(Stream SourceGif, BinaryWriter Writer)
        {
            SourceGif.Position = 13; // Locating the image color table
            var colorTable = new byte[768];
            SourceGif.Read(colorTable, 0, colorTable.Length);
            Writer.Write(colorTable, 0, colorTable.Length);
        }

        static void WriteGraphicControlBlock(Stream SourceGif, BinaryWriter Writer, int FrameDelay)
        {
            SourceGif.Position = 781; // Locating the source GCE
            var blockhead = new byte[8];
            SourceGif.Read(blockhead, 0, blockhead.Length); // Reading source GCE

            Writer.Write(unchecked((short)0xf921)); // Identifier
            Writer.Write((byte)0x04); // Block Size
            Writer.Write((byte)(blockhead[3] & 0xf7 | 0x08)); // Setting disposal flag
            Writer.Write((short)(FrameDelay / 10)); // Setting frame delay
            Writer.Write(blockhead[6]); // Transparent color index
            Writer.Write((byte)0); // Terminator
        }

        static byte[] _buffer;
        static readonly byte[] Header = new byte[11];

        static void WriteImageBlock(Stream SourceGif, BinaryWriter Writer, bool IncludeColorTable, int X, int Y, int Width, int Height)
        {
            SourceGif.Position = SourceImageBlockPosition; // Locating the image block
            
            SourceGif.Read(Header, 0, Header.Length);
            Writer.Write(Header[0]); // Separator
            Writer.Write((short)X); // Position X
            Writer.Write((short)Y); // Position Y
            Writer.Write((short)Width); // Width
            Writer.Write((short)Height); // Height

            if (IncludeColorTable) // If first frame, use global color table - else use local
            {
                SourceGif.Position = SourceGlobalColorInfoPosition;
                Writer.Write((byte)(SourceGif.ReadByte() & 0x3f | 0x80)); // Enabling local color table
                WriteColorTable(SourceGif, Writer);
            }
            else Writer.Write((byte)(Header[9] & 0x07 | 0x07)); // Disabling local color table

            Writer.Write(Header[10]); // LZW Min Code Size

            // Read/Write image data
            SourceGif.Position = SourceImageBlockPosition + Header.Length;

            var dataLength = SourceGif.ReadByte();
            while (dataLength > 0)
            {
                if (_buffer == null || _buffer.Length < dataLength)
                    _buffer = new byte[dataLength];
                                                
                SourceGif.Read(_buffer, 0, dataLength);
                
                Writer.Write((byte)dataLength);
                Writer.Write(_buffer, 0, dataLength);
                dataLength = SourceGif.ReadByte();
            }

            Writer.Write((byte)0); // Terminator

            Writer.Flush();
        }
        #endregion

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            // Complete File
            _writer.Write((byte)0x3b); // File Trailer

            _writer.BaseStream.Dispose();
            _writer.Dispose();

            _gifStream.Dispose();
        }
    }
}
