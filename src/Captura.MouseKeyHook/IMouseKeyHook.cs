using System;
using System.Windows.Forms;

namespace Captura.MouseKeyHook
{
    public interface IMouseKeyHook : IDisposable
    {
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseClick;
        event MouseEventHandler MouseDoubleClick;
        event MouseEventHandler MouseWheel;
        event MouseEventHandler MouseMove;

        event MouseEventHandler MouseDragStarted;
        event MouseEventHandler MouseDragFinished;

        event KeyEventHandler KeyUp;
        event KeyEventHandler KeyDown;
        event KeyPressEventHandler KeyPress;
    }
}