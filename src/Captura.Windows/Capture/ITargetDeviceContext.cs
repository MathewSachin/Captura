using System;

namespace Captura.Video
{
    interface ITargetDeviceContext : IDisposable
    {
        IntPtr GetDC();

        IBitmapFrame DummyFrame { get; }

        IEditableFrame GetEditableFrame();
    }
}
