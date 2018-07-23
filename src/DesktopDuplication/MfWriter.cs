using System;
using System.Diagnostics;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class MfWriter : IDisposable
    {
        const int BitRate = 4_000_000;
        readonly Guid _encodingFormat = VideoFormatGuids.H264;
        readonly Guid _inputFormat = VideoFormatGuids.Rgb32;

        readonly Stopwatch _stopwatch = new Stopwatch();

        long _prevFrameTicks;

        readonly SinkWriter _writer;
        readonly int _streamIndex;

        static long PackLong(int Left, int Right)
        {
            //implicit conversion of left to a long
            long res = Left;

            //shift the bits creating an empty space on the right
            // ex: 0x0000CFFF becomes 0xCFFF0000
            res = (res << 32);

            //combine the bits on the right with the previous value
            // ex: 0xCFFF0000 | 0x0000ABCD becomes 0xCFFFABCD
            res = res | (uint)Right; //uint first to prevent loss of signed bit

            //return the combined result
            return res;
        }

        public MfWriter(Device Device, int Fps, int Width, int Height, string FileName)
        {
            var attr = new MediaAttributes(3);
            attr.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1);
            attr.Set(SinkWriterAttributeKeys.LowLatency, true);

            var devMan = new DXGIDeviceManager();
            devMan.ResetDevice(Device);
            attr.Set(SinkWriterAttributeKeys.D3DManager, devMan);

            _writer = MediaFactory.CreateSinkWriterFromURL(FileName, null, attr);

            using (var mediaTypeOut = new MediaType())
            {
                mediaTypeOut.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeOut.Set(MediaTypeAttributeKeys.Subtype, _encodingFormat);
                mediaTypeOut.Set(MediaTypeAttributeKeys.AvgBitrate, BitRate);
                mediaTypeOut.Set(MediaTypeAttributeKeys.InterlaceMode, (int) VideoInterlaceMode.Progressive);
                mediaTypeOut.Set(MediaTypeAttributeKeys.FrameSize, PackLong(Width, Height));
                mediaTypeOut.Set(MediaTypeAttributeKeys.FrameRate, PackLong(Fps, 1));
                mediaTypeOut.Set(MediaTypeAttributeKeys.PixelAspectRatio, PackLong(1, 1));

                _writer.AddStream(mediaTypeOut, out _streamIndex);
            }

            using (var mediaTypeIn = new MediaType())
            {
                mediaTypeIn.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeIn.Set(MediaTypeAttributeKeys.Subtype, _inputFormat);
                mediaTypeIn.Set(MediaTypeAttributeKeys.InterlaceMode, (int) VideoInterlaceMode.Progressive);
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameSize, PackLong(Width, Height));
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameRate, PackLong(Fps, 1));
                mediaTypeIn.Set(MediaTypeAttributeKeys.PixelAspectRatio, PackLong(1, 1));

                _writer.SetInputMediaType(_streamIndex, mediaTypeIn, null);
            }

            _writer.BeginWriting();

            _stopwatch.Start();
        }

        bool _first = true;

        readonly object _syncLock = new object();

        public void Write(Sample Sample)
        {
            lock (_syncLock)
            {
                var nowTicks = _stopwatch.ElapsedTicks;

                if (_prevFrameTicks == 0)
                    _prevFrameTicks = nowTicks;

                Sample.SampleTime = _prevFrameTicks;
                Sample.SampleDuration = nowTicks - _prevFrameTicks;

                _prevFrameTicks = nowTicks;
            
                if (_disposed)
                    return;

                if (_first)
                {
                    _writer.SendStreamTick(_streamIndex, nowTicks);

                    Sample.Set(SampleAttributeKeys.Discontinuity, true);

                    _first = false;
                }

                _writer.WriteSample(_streamIndex, Sample);
            }
        }

        bool _disposed;

        public void Dispose()
        {
            lock (_syncLock)
            {
                _disposed = true;

                _writer.Finalize();
                _writer.Dispose();

                _stopwatch.Stop();
            }
        }
    }
}