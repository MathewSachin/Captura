using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace Captura.MouseKeyHook
{
    public class MouseKeyHook : IMouseKeyHook
    {
        readonly IKeyboardMouseEvents _hook;

        public MouseKeyHook()
        {
            _hook = Hook.GlobalEvents();
        }

        public event MouseEventHandler MouseUp
        {
            add => _hook.MouseUp += value;
            remove => _hook.MouseUp -= value;
        }

        public event MouseEventHandler MouseDown
        {
            add => _hook.MouseDown += value;
            remove => _hook.MouseDown -= value;
        }

        public event MouseEventHandler MouseClick
        {
            add => _hook.MouseClick += value;
            remove => _hook.MouseClick -= value;
        }

        public event MouseEventHandler MouseDoubleClick
        {
            add => _hook.MouseDoubleClick += value;
            remove => _hook.MouseDoubleClick -= value;
        }

        public event MouseEventHandler MouseWheel
        {
            add => _hook.MouseWheel += value;
            remove => _hook.MouseWheel -= value;
        }

        public event MouseEventHandler MouseMove
        {
            add => _hook.MouseMove += value;
            remove => _hook.MouseMove -= value;
        }

        public event MouseEventHandler MouseDragStarted
        {
            add => _hook.MouseDragStarted += value;
            remove => _hook.MouseDragStarted -= value;
        }

        public event MouseEventHandler MouseDragFinished
        {
            add => _hook.MouseDragFinished += value;
            remove => _hook.MouseDragFinished -= value;
        }

        public event KeyEventHandler KeyUp
        {
            add => _hook.KeyUp += value;
            remove => _hook.KeyUp -= value;
        }

        public event KeyEventHandler KeyDown
        {
            add => _hook.KeyDown += value;
            remove => _hook.KeyDown -= value;
        }

        public event KeyPressEventHandler KeyPress
        {
            add => _hook.KeyPress += value;
            remove => _hook.KeyPress -= value;
        }

        public void Dispose()
        {
            _hook.Dispose();
        }
    }
}
