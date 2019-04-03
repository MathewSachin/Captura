using System;
using System.Windows.Forms;

namespace Captura.Models
{
    public interface IMouseKeyHook : IDisposable
    {
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseDown;

        event KeyEventHandler KeyUp;
        event KeyEventHandler KeyDown;
    }
}