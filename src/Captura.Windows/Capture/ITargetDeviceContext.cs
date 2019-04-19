using System;
using Captura;

namespace Screna
{
    interface ITargetDeviceContext : IDisposable
    {
        IntPtr GetDC();

        Type EditorType { get; }

        IEditableFrame GetEditableFrame();
    }
}
