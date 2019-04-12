using System;
using System.Diagnostics;
using Captura;
using Screna;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using Device = SharpDX.Direct3D11.Device;

namespace DesktopDuplication
{
    public class MfWriter : IVideoFileWriter
    {
        readonly Device _device;
        const int BitRate = 8_000_000;
        readonly Guid _encodingFormat = VideoFormatGuids.H264;
        readonly Guid _inputFormat = VideoFormatGuids.NV12;
        readonly Stopwatch _stopwatch = new Stopwatch();
        readonly long _frameDuration;
        readonly SinkWriter _writer;
        readonly int _streamIndex;

        static long PackLong(int Left, int Right)
        {
            return new PackedLong
            {
                Low = Right,
                High = Left
            }.Long;
        }

        public MfWriter(int Fps, int Width, int Height, string FileName, Device Device)
        {
            _device = Device;

            _frameDuration = 10_000_000 / Fps;

            var attr = new MediaAttributes(6);

            attr.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1);
            attr.Set(SinkWriterAttributeKeys.ReadwriteDisableConverters, 0);
            attr.Set(TranscodeAttributeKeys.TranscodeContainertype, TranscodeContainerTypeGuids.Mpeg4);
            attr.Set(SinkWriterAttributeKeys.LowLatency, true);
            attr.Set(SinkWriterAttributeKeys.DisableThrottling, 1);

            var devMan = new DXGIDeviceManager();
            devMan.ResetDevice(Device);
            attr.Set(SinkWriterAttributeKeys.D3DManager, devMan);

            _writer = MediaFactory.CreateSinkWriterFromURL(FileName, null, attr);

            using (var mediaTypeOut = new MediaType())
            {
                mediaTypeOut.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeOut.Set(MediaTypeAttributeKeys.Subtype, _encodingFormat);
                mediaTypeOut.Set(MediaTypeAttributeKeys.AvgBitrate, BitRate);
                mediaTypeOut.Set(MediaTypeAttributeKeys.InterlaceMode, (int)VideoInterlaceMode.Progressive);
                mediaTypeOut.Set(MediaTypeAttributeKeys.FrameSize, PackLong(Width, Height));
                mediaTypeOut.Set(MediaTypeAttributeKeys.FrameRate, PackLong(Fps, 1));
                mediaTypeOut.Set(MediaTypeAttributeKeys.PixelAspectRatio, PackLong(1, 1));
                _writer.AddStream(mediaTypeOut, out _streamIndex);
            }

            using (var mediaTypeIn = new MediaType())
            {
                mediaTypeIn.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeIn.Set(MediaTypeAttributeKeys.Subtype, _inputFormat);
                mediaTypeIn.Set(MediaTypeAttributeKeys.InterlaceMode, (int)VideoInterlaceMode.Progressive);
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameSize, PackLong(Width, Height));
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameRate, PackLong(Fps, 1));
                mediaTypeIn.Set(MediaTypeAttributeKeys.PixelAspectRatio, PackLong(1, 1));
                _writer.SetInputMediaType(_streamIndex, mediaTypeIn, null);
            }

            _writer.BeginWriting();
            _stopwatch.Start();

            _copyTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });

            _sample = MediaFactory.CreateVideoSampleFromSurface(null);
                
            // Create the media buffer from the texture
            MediaFactory.CreateDXGISurfaceBuffer(typeof(Texture2D).GUID, _copyTexture, 0, false, out _mediaBuffer);

            using (var buffer2D = _mediaBuffer.QueryInterface<Buffer2D>())
                _mediaBuffer.CurrentLength = buffer2D.ContiguousLength;

            // Attach the created buffer to the sample
            _sample.AddBuffer(_mediaBuffer);
        }

        bool _first = true;

        readonly object _syncLock = new object();

        public void Write(Sample Sample)
        {
            lock (_syncLock)
            {
                if (_disposed)
                    return;

                Sample.SampleTime = _stopwatch.Elapsed.Ticks;
                Sample.SampleDuration = _frameDuration;
                
                if (_first)
                {
                    _writer.SendStreamTick(_streamIndex, Sample.SampleTime);
                    Sample.Set(SampleAttributeKeys.Discontinuity, true);
                    _first = false;
                }
                _writer.WriteSample(_streamIndex, Sample);
            }
        }

        Texture2D _copyTexture;
        Sample _sample;
        MediaBuffer _mediaBuffer;

        void Write(Texture2D Texture)
        {
            _device.ImmediateContext.CopyResource(Texture, _copyTexture);

            Write(_sample);
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

                _copyTexture.Dispose();
                _copyTexture = null;

                _sample.Dispose();
                _sample = null;

                _mediaBuffer.Dispose();
                _mediaBuffer = null;
            }
        }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is Texture2DFrame frame)
            {
                Write(frame.Texture);
            }
            else if (Image is MultiDisposeFrame wrapper && wrapper.Frame is Texture2DFrame textureFrame)
            {
                Write(textureFrame.Texture);
            }
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Length)
        {
        }
    }
}