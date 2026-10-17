using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Captura.Video;
using Captura.Windows.DesktopDuplication;
using Captura.Windows.DirectX;
using Moq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Xunit;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class DuplCaptureTests
    {
        [Fact]
        public void CapturedFramesReachEditorTexture()
        {
            using var factory = new Factory1();
            var adapters = factory.Adapters1;
            var outputs = adapters.SelectMany(A => A.Outputs).ToArray();

            try
            {
                Assert.NotEmpty(outputs);

                using var output = outputs[0].QueryInterface<Output1>();
                var bounds = output.Description.DesktopBounds;

                using var session = new Direct2DEditorSession(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top, Mock.Of<IPreviewWindow>());
                using var pointer = new DxMousePointer(session);
                using var duplCapture = new DuplCapture(output);

                Assert.False(duplCapture.Get(session.DesktopTexture, pointer), "Copied a frame before any was captured");

                for (var i = 0; i < 60; ++i)
                {
                    duplCapture.Get(session.DesktopTexture, pointer);

                    Thread.Sleep(16);
                }

                Assert.Equal(Result.Ok, session.Device.DeviceRemovedReason);

                session.BeginDraw();
                session.EndDraw();

                var desc = session.StagingTexture.Description;
                var pixels = new byte[desc.Width * desc.Height * 4];
                var map = session.Device.ImmediateContext.MapSubresource(session.StagingTexture, 0, MapMode.Read, MapFlags.None);

                try
                {
                    for (var y = 0; y < desc.Height; ++y)
                    {
                        Marshal.Copy(map.DataPointer + y * map.RowPitch, pixels, y * desc.Width * 4, desc.Width * 4);
                    }
                }
                finally
                {
                    session.Device.ImmediateContext.UnmapSubresource(session.StagingTexture, 0);
                }

                Assert.True(pixels.Any(B => B != 0), "Captured frame is empty");
            }
            finally
            {
                foreach (var comObject in outputs.Concat<IDisposable>(adapters))
                {
                    comObject.Dispose();
                }
            }
        }
    }
}
