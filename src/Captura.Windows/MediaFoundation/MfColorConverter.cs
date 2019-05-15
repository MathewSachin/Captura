using DesktopDuplication;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using System;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D11.Device;

namespace Captura
{
    public class MfColorConverter : IDisposable
    {
        Transform _colorConverter;
        DXGIDeviceManager _deviceMan;

        public MfColorConverter(int Width, int Height, Device Device)
        {
            var transforms = MediaFactory.FindTransform(TransformCategoryGuids.VideoProcessor, TransformEnumFlag.All);
            _colorConverter = transforms[0].ActivateObject<Transform>();

            _deviceMan = new DXGIDeviceManager();
            _deviceMan.ResetDevice(Device);

            // TODO: Works without this line.
            //_colorConverter.ProcessMessage(TMessageType.SetD3DManager, _deviceMan.NativePointer);

            using (var mediaTypeIn = new MediaType())
            {
                mediaTypeIn.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeIn.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);
                mediaTypeIn.Set(MediaTypeAttributeKeys.FrameSize, MfWriter.PackLong(Width, Height));
                mediaTypeIn.Set(MediaTypeAttributeKeys.DefaultStride, Width * 4);
                mediaTypeIn.Set(MediaTypeAttributeKeys.FixedSizeSamples, 1);
                mediaTypeIn.Set(MediaTypeAttributeKeys.SampleSize, Width * Height * 4);

                _colorConverter.SetInputType(0, mediaTypeIn, 0);
            }

            var outputStride = Width * 12 / 8;
            var outputSampleSize = Height * outputStride;

            using (var mediaTypeOut = new MediaType())
            {
                mediaTypeOut.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                mediaTypeOut.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.NV12);
                mediaTypeOut.Set(MediaTypeAttributeKeys.FrameSize, MfWriter.PackLong(Width, Height));
                mediaTypeOut.Set(MediaTypeAttributeKeys.DefaultStride, outputStride);
                mediaTypeOut.Set(MediaTypeAttributeKeys.FixedSizeSamples, 1);
                mediaTypeOut.Set(MediaTypeAttributeKeys.SampleSize, outputSampleSize);

                _colorConverter.SetOutputType(0, mediaTypeOut, 0);
            }

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

            _inputSample = MediaFactory.CreateVideoSampleFromSurface(null);

            // Create the media buffer from the texture
            MediaFactory.CreateDXGISurfaceBuffer(typeof(Texture2D).GUID, _copyTexture, 0, false, out var inputBuffer);

            using (var buffer2D = inputBuffer.QueryInterface<Buffer2D>())
                inputBuffer.CurrentLength = buffer2D.ContiguousLength;

            // Attach the created buffer to the sample
            _inputSample.AddBuffer(inputBuffer);

            _outputTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.NV12,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });

            _outputSample = MediaFactory.CreateVideoSampleFromSurface(null);

            _outputSample.SampleDuration = 1;

            MediaFactory.CreateDXGISurfaceBuffer(typeof(Texture2D).GUID, _outputTexture, 0, false, out _outBuffer);

            using (var buffer2D = _outBuffer.QueryInterface<Buffer2D>())
                _outBuffer.CurrentLength = buffer2D.ContiguousLength;

            _outputSample.AddBuffer(_outBuffer);

            _outDataBuffer = new TOutputDataBuffer[1];
            _outDataBuffer[0].PSample = _outputSample;
            MediaFactory.CreateCollection(out _outDataBuffer[0].PEvents);
        }

        int _frameNumber;

        Texture2D _copyTexture, _outputTexture;
        Sample _inputSample, _outputSample;
        MediaBuffer _outBuffer;
        TOutputDataBuffer[] _outDataBuffer;

        public void Convert(Texture2D Texture, byte[] Output)
        {
            Texture.Device.ImmediateContext.CopyResource(Texture, _copyTexture);

            if (_frameNumber == 0)
            {
                _inputSample.Set(SampleAttributeKeys.Discontinuity, true);
            }

            _inputSample.SampleTime = _frameNumber++ * 1_000_000;

            _colorConverter.ProcessInput(0, _inputSample, 0);

            _colorConverter.ProcessOutput(0, _outDataBuffer, out _);

            var ptr = _outBuffer.Lock(out _, out _);

            Marshal.Copy(ptr, Output, 0, Output.Length);

            _outBuffer.Unlock();
        }

        public void Dispose()
        {
            _inputSample.Dispose();
            _outputSample.Dispose();

            _outBuffer.Dispose();

            _copyTexture.Dispose();
            _outputTexture.Dispose();

            _colorConverter.Dispose();
            _colorConverter = null;

            _deviceMan.Dispose();
        }
    }
}
