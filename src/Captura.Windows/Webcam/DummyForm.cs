using System;
using System.Windows.Forms;

namespace Captura.Webcam
{
    class DummyForm : Form
    {
        public DummyForm()
        {
            Opacity = 0;
            ShowInTaskbar = false;
        }

        protected override void WndProc(ref Message M)
        {
            const int msgLeftButtonDown = 513;

            if (M.Msg == msgLeftButtonDown)
            {
                OnClick(EventArgs.Empty);
            }

            base.WndProc(ref M);
        }
    }
}