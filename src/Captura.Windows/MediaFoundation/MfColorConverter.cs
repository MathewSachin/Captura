using MediaFoundation.Transform;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using System;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D11.Device;

namespace Captura.Windows.MediaFoundation
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

            _colorConverter.ProcessMessage(TMessageType.SetD3DManager, _deviceMan.NativePointer);

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

            _colorConverter.ProcessMessage(TMessageType.NotifyBeginStreaming, IntPtr.Zero);

            _copyTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });

            _inputSample = MediaFactory.CreateVideoSampleFromSurface(null);

            // Create the media buffer from the texture
            MediaFactory.CreateDXGISurfaceBuffer(typeof(Texture2D).GUID, _copyTexture, 0, false, out var inputBuffer);

            using (var buffer2D = inputBuffer.QueryInterface<Buffer2D>())
                inputBuffer.CurrentLength = buffer2D.ContiguousLength;

            // Attach the created buffer to the sample
            _inputSample.AddBuffer(inputBuffer);
        }

        int _frameNumber;

        Texture2D _copyTexture;
        Sample _inputSample;

        public void Convert(Texture2D Texture, byte[] Output)
        {
            Texture.Device.ImmediateContext.CopyResource(Texture, _copyTexture);

            _inputSample.SampleTime = _frameNumber++ * 1_000_000;

            _colorConverter.ProcessInput(0, _inputSample, 0);

            var buf = new MFTOutputDataBuffer[1];

            // HACK: Need to use Media Foundation .NET here due to bug in SharpDX.MediaFoundation (no longer maintained).
            // I think the output data buffer is not [Out] marshalled.
            ((IMFTransform)Marshal.GetObjectForIUnknown(_colorConverter.NativePointer)).ProcessOutput(0, 1, buf, out _);

            using var sample = new Sample(buf[0].pSample);
            using var buffer = sample.GetBufferByIndex(0);
            var ptr = buffer.Lock(out _, out _);

            Marshal.Copy(ptr, Output, 0, Output.Length);

            buffer.Unlock();
        }

        public void Dispose()
        {
            _colorConverter.ProcessMessage(TMessageType.NotifyEndOfStream, IntPtr.Zero);

            _inputSample.Dispose();

            _copyTexture.Dispose();

            _colorConverter.Dispose();
            _colorConverter = null;

            _deviceMan.Dispose();
        }
    }
}
