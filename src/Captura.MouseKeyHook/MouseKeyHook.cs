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
        }

        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseDown;

        public event KeyEventHandler KeyUp;
        public event KeyEventHandler KeyDown;

        public void Dispose()
        {
            _hook.Dispose();

            KeyUp = KeyDown = null;
            MouseUp = MouseDown = null;
        }
    }
}
