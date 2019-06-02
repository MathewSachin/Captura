using System;
using Captura;

namespace Screna
{
    interface ITargetDeviceContext : IDisposable
    {
        IntPtr GetDC();

        IBitmapFrame DummyFrame { get; }

        IEditableFrame GetEditableFrame();
    }
}
