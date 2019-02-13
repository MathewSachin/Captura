using System;
using System.Drawing;
using System.Windows.Forms;
using Captura;
using Captura.Models;
using Captura.Native;
using User32 = Captura.Native.User32;

namespace Screna
{
    class WindowScreenShotBackdrop : IDisposable
    {
        readonly IWindow _window;
        readonly Form _form;

        bool _shown;

        public Rectangle Rectangle { get; }

        public WindowScreenShotBackdrop(IWindow Window)
        {
            _window = Window;

            // Show and Focus
            User32.ShowWindow(Window.Handle, 5);

            _form = new Form
            {
                AllowTransparency = true,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false
            };

            var r = Window.Rectangle;

            // Add a margin for window shadows. Excess transparency is trimmed out later
            r.Inflate(20, 20);

            // Check if the window is outside of the visible screen
            var platformServices = ServiceProvider.Get<IPlatformServices>();

            r.Intersect(platformServices.DesktopRectangle);

            Rectangle = r;
        }

        void Show()
        {
            if (_shown)
                return;

            _shown = true;

            _form.Show();

            User32.SetWindowPos(_form.Handle, _window.Handle,
                Rectangle.Left, Rectangle.Top,
                Rectangle.Width, Rectangle.Height,
                SetWindowPositionFlags.NoActivate);
        }

        public void ShowWhite()
        {
            Show();

            _form.BackColor = Color.White;

            // Wait for Backdrop to update
            Application.DoEvents();
        }

        public void ShowBlack()
        {
            Show();

            _form.BackColor = Color.Black;

            // Wait for Backdrop to update
            Application.DoEvents();
        }

        public void Dispose()
        {
            _form.Dispose();
        }
    }
}