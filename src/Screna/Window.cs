using Screna.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Screna
{
    /// <summary>
    /// Minimal representation of a Window.
    /// </summary>
    public class Window
    {
        #region PInvoke
        const string DllName = "user32.dll";
        
        [DllImport(DllName)] 
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport(DllName)]
        static extern IntPtr GetDesktopWindow();

        [DllImport(DllName)]
        static extern IntPtr GetForegroundWindow();

        [DllImport(DllName)]
        static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(DllName)]
        static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport(DllName)]
        static extern IntPtr GetWindow(IntPtr hWnd, GetWindowEnum uCmd);

        [DllImport(DllName)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport(DllName)]
        static extern bool IsWindowVisible(IntPtr hWnd);
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="Window"/>.
        /// </summary>
        /// <param name="Handle">The Window Handle.</param>
        public Window(IntPtr Handle)
        {
            if (!IsWindow(Handle))
                throw new ArgumentException("Not a Window.", nameof(Handle));

            this.Handle = Handle;
        }

        /// <summary>
        /// Gets whether the Window is Visible.
        /// </summary>
        public bool IsVisible => IsWindowVisible(Handle);

        /// <summary>
        /// Gets the Window Handle.
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// Gets the Window Title.
        /// </summary>
        public string Title
        {
            get
            {
                var title = new StringBuilder(GetWindowTextLength(Handle) + 1);
                GetWindowText(Handle, title, title.Capacity);
                return title.ToString();
            }
        }

        /// <summary>
        /// Get the Window Rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                if (User32.GetWindowRect(Handle, out var rect))
                {
                    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }

                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Gets the Desktop Window.
        /// </summary>
        public static Window DesktopWindow { get; } = new Window(GetDesktopWindow());

        /// <summary>
        /// Gets the Foreground Window.
        /// </summary>
        public static Window ForegroundWindow => new Window(GetForegroundWindow());
        
        /// <summary>
        /// Gets the Taskbar Window - Shell_TrayWnd.
        /// </summary>
        public static Window Taskbar { get; } = new Window(User32.FindWindow("Shell_TrayWnd", null));

        /// <summary>
        /// Enumerates all Windows.
        /// </summary>
        public static IEnumerable<Window> Enumerate()
        {
            var list = new List<Window>();

            EnumWindows((hWnd, lParam) =>
            {
                var wh = new Window(hWnd);

                list.Add(wh);

                return true;
            }, IntPtr.Zero);

            return list;
        }

        /// <summary>
        /// Enumerates all visible windows with a Title.
        /// </summary>
        public static IEnumerable<Window> EnumerateVisible()
        {
            foreach (var hWnd in Enumerate().Where(W => W.IsVisible && !string.IsNullOrWhiteSpace(W.Title))
                                            .Select(W => W.Handle))
            {
                if (!User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.AppWindow))
                {
                    if (GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.ToolWindow))
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.Style).HasFlag(WindowStyles.Child))
                        continue;
                }

                yield return new Window(hWnd);
            }
        }

        /// <summary>
        /// Returns the Widow Title.
        /// </summary>
        public override string ToString() => Title;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) => obj is Window w && w.Handle == Handle;
        
        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() => Handle.GetHashCode();

        /// <summary>
        /// Checks whether two <see cref="Window"/> instances are equal.
        /// </summary>
        public static bool operator ==(Window W1, Window W2) => W1.Handle == W2.Handle;

        /// <summary>
        /// Checks whether two <see cref="Window"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Window W1, Window W2) => !(W1 == W2);
    }
}