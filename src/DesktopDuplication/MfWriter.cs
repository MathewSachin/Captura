using System;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class MfWriter : IDisposable
    {
        const int BitRate = 1_000_000;
        readonly Guid _encodingFormat = VideoFormatGuids.H264;
        readonly Guid _inputFormat = VideoFormatGuids.Rgb32;

        long _prevFrameTicks;

        readonly SinkWriter _writer;
        readonly int _streamIndex;

        static long PackLong(int Low, int High)
        {
            return ((uint)Low << 32) | (uint)High;
        }

        public MfWriter(Device Device, int Fps, int Width, int Height)
        {
            var attr = new MediaAttributes(3);
            attr.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1);
            attr.Set(SinkWriterAttributeKeys.LowLatency, true);

            var devMan = new DXGIDeviceManager();
            devMan.ResetDevice(Device);
            attr.Set(SinkWriterAttributeKeys.D3DManager, devMan);

            _writer = MediaFactory.CreateSinkWriterFromURL(@"output.mp4", null, attr);

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
                var mediaAttr = new MediaAttributes();

                mediaTypeIn.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeIn.Set(MediaTypeAttributeKeys.Subtype, _inputFormat);
                mediaTypeIn.Set(MediaTypeAttributeKeys.InterlaceMode, (int) VideoInterlaceMode.Progressive);
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameSize, PackLong(Width, Height));
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameRate, PackLong(Fps, 1));
                mediaTypeIn.Set(MediaTypeAttributeKeys.PixelAspectRatio, PackLong(1, 1));

                _writer.SetInputMediaType(_streamIndex, mediaTypeIn, null);
            }

            _writer.BeginWriting();
        }

        bool _first = true;

        public void Write(Sample Sample)
        {
            var nowTicks = DateTime.Now.Ticks;

            if (_prevFrameTicks == 0)
                _prevFrameTicks = nowTicks;

            Sample.SampleTime = _prevFrameTicks;
            Sample.SampleDuration = nowTicks - _prevFrameTicks;

            _prevFrameTicks = nowTicks;

            if (_first)
            {
                _writer.SendStreamTick(_streamIndex, nowTicks);

                Sample.Set(SampleAttributeKeys.Discontinuity, true);

                _first = false;
            }

            _writer.WriteSample(_streamIndex, Sample);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}