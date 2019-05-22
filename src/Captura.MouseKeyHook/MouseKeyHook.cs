using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace Captura.Models
{
    class MouseKeyHook : IMouseKeyHook
    {
        readonly IKeyboardMouseEvents _hook;

        public MouseKeyHook()
        {
            _hook = Hook.GlobalEvents();

            _hook.KeyUp += (S, E) => KeyUp?.Invoke(this, E);
            _hook.KeyDown += (S, E) => KeyDown?.Invoke(this, E);

            _hook.MouseUp += (S, E) => MouseUp?.Invoke(this, E);
            _hook.MouseDown += (S, E) => MouseDown?.Invoke(this, E);
            _hook.MouseClick += (S, E) => MouseClick?.Invoke(this, E);
            _hook.MouseDoubleClick += (S, E) => MouseDoubleClick?.Invoke(this, E);
            _hook.MouseWheel += (S, E) => MouseWheel?.Invoke(this, E);
            _hook.MouseMove += (S, E) => MouseMove?.Invoke(this, E);

            _hook.MouseDragStarted += (S, E) => MouseDragStarted?.Invoke(this, E);
            _hook.MouseDragFinished += (S, E) => MouseDragFinished?.Invoke(this, E);

            _hook.KeyPress += (S, E) => KeyPress?.Invoke(this, E);
        }

        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDoubleClick;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler MouseMove;

        public event MouseEventHandler MouseDragStarted;
        public event MouseEventHandler MouseDragFinished;

        public event KeyEventHandler KeyUp;
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;

        public void Dispose()
        {
            _hook.Dispose();
        }
    }
}
