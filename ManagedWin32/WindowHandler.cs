using System;
using System.Collections.Generic;
using System.Text;

namespace Captura
{
    public class WindowHandler
    {
        WindowHandler(IntPtr hWnd) { this.Handle = hWnd; }

        public bool IsVisible { get { return User32.IsWindowVisible(Handle); } }

        public IntPtr Handle { get; private set; }

        public string Title
        {
            get
            {
                StringBuilder title = new StringBuilder(User32.GetWindowTextLength(Handle) + 1);
                User32.GetWindowText(Handle, title, title.Capacity);
                return title.ToString();
            }
        }

        public static IEnumerable<WindowHandler> Enumerate()
        {
            List<WindowHandler> list = new List<WindowHandler>();

            User32.EnumWindows((hWnd, lParam) =>
                {
                    list.Add(new WindowHandler(hWnd));

                    return true;
                }, IntPtr.Zero);

            return list;
        }
    }
}