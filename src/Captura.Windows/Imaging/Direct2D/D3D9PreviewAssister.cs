using System;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;

// Adapted from https://github.com/Marlamin/SharpDX.WPF

namespace Captura.Windows.DirectX
{
    public class D3D9PreviewAssister : IDisposable
    {
        readonly Direct3DEx _direct3D;
        readonly DeviceEx _device;

        public D3D9PreviewAssister(IPlatformServices PlatformServices)
        {
            _direct3D = new Direct3DEx();

            var presentparams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = PlatformServices.DesktopWindow.Handle,
                PresentationInterval = PresentInterval.Default
            };

            _device = new DeviceEx(_direct3D,
                0,
                DeviceType.Hardware,
                IntPtr.Zero,
                CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
                presentparams);
        }

        public Texture GetSharedTexture(Texture2D Texture)
        {
            return GetSharedD3D9(_device, Texture);
        }

        // Texture must be created with ResourceOptionFlags.Shared
        // Texture format must be B8G8R8A8_UNorm
        static Texture GetSharedD3D9(DeviceEx Device, Texture2D RenderTarget)
        {
            using var resource = RenderTarget.QueryInterface<SharpDX.DXGI.Resource>();
            var handle = resource.SharedHandle;

            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));

            return new Texture(Device,
                RenderTarget.Description.Width,
                RenderTarget.Description.Height,
                1,
                Usage.RenderTarget,
                Format.A8R8G8B8,
                Pool.Default,
                ref handle);
        }

        public void Dispose()
        {
            _device.Dispose();
            _direct3D.Dispose();
        }
    }
}