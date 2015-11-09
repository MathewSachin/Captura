using System;
using System.Drawing;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using ManagedWin32.Api;

namespace Captura
{
    class MouseKeyHookFacade : IDisposable
    {
        IKeyboardMouseEvents ClickHook;

        bool MouseClicked;

        Keys LastKeyPressed;

        public MouseKeyHookFacade(bool CaptureMouseClicks, bool CaptureKeystrokes)
        {
            LastKeyPressed = Keys.None;
            MouseClicked = false;

            ClickHook = Hook.GlobalEvents();

            if (CaptureMouseClicks) ClickHook.MouseDown += (s, e) => MouseClicked = true;
            if (CaptureKeystrokes) ClickHook.KeyDown += (s, e) => LastKeyPressed = e.KeyCode;
        }

        public void Draw(Graphics g)
        {
            if (MouseClicked)
            {
                var curPos = User32.CursorPosition;
                g.DrawArc(new Pen(Color.Black, 1), curPos.X - 40, curPos.Y - 40, 80, 80, 0, 360);

                MouseClicked = false;
            }

            if (LastKeyPressed != Keys.None)
            {
                g.DrawString(LastKeyPressed.ToString(),
                    new Font(FontFamily.GenericMonospace, 100),
                    new SolidBrush(Color.Black), 100, 100);

                LastKeyPressed = Keys.None;
            }
        }

        public void Dispose()
        {
            ClickHook.Dispose();
            ClickHook = null;
        }
    }
}
