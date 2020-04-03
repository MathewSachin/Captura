using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Captura.Native;

namespace Captura.Video
{
    /// <summary>
    /// Minimal representation of a Window.
    /// </summary>
    class Window : IWindow
    {
        /// <summary>
        /// Creates a new instance of <see cref="Window"/>.
        /// </summary>
        /// <param name="Handle">The Window Handle.</param>
        public Window(IntPtr Handle)
        {
            if (!User32.IsWindow(Handle))
                throw new ArgumentException("Not a Window.", nameof(Handle));

            this.Handle = Handle;
        }

        public bool IsAlive => User32.IsWindow(Handle);

        /// <summary>
        /// Gets whether the Window is Visible.
        /// </summary>
        public bool IsVisible => User32.IsWindowVisible(Handle);

        public bool IsMaximized => User32.IsZoomed(Handle);

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
                var title = new StringBuilder(User32.GetWindowTextLength(Handle) + 1);
                User32.GetWindowText(Handle, title, title.Capacity);
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
                var r = new RECT();

                const int extendedFrameBounds = 9;

                if (DwmApi.DwmGetWindowAttribute(Handle, extendedFrameBounds, ref r, Marshal.SizeOf<RECT>()) != 0)
                {
                    if (!User32.GetWindowRect(Handle, out r))
                        return Rectangle.Empty;
                }

                return r.ToRectangle();
            }
        }

        /// <summary>
        /// Gets the Desktop Window.
        /// </summary>
        public static Window DesktopWindow { get; } = new Window(User32.GetDesktopWindow());

        /// <summary>
        /// Gets the Foreground Window.
        /// </summary>
        public static Window ForegroundWindow => new Window(User32.GetForegroundWindow());

        /// <summary>
        /// Enumerates all Windows.
        /// </summary>
        public static IEnumerable<Window> Enumerate()
        {
            var list = new List<Window>();

            User32.EnumWindows((Handle, Param) =>
            {
                var wh = new Window(Handle);

                list.Add(wh);

                return true;
            }, IntPtr.Zero);

            return list;
        }

        public IEnumerable<Window> EnumerateChildren()
        {
            var list = new List<Window>();

            User32.EnumChildWindows(Handle, (Handle, Param) =>
            {
                var wh = new Window(Handle);

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
            foreach (var window in Enumerate().Where(W => W.IsVisible && !string.IsNullOrWhiteSpace(W.Title)))
            {
                var hWnd = window.Handle;

                if (!User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.AppWindow))
                {
                    if (User32.GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.ToolWindow))
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.Style).HasFlag(WindowStyles.Child))
                        continue;
                }

                const int dwmCloaked = 14;

                // Exclude suspended Windows apps
                DwmApi.DwmGetWindowAttribute(hWnd, dwmCloaked, out var cloaked, Marshal.SizeOf<bool>());

                if (cloaked)
                    continue;

                yield return window;
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
        /// <param name="Obj">The object to compare with the current object. </param>
        public override bool Equals(object Obj) => Obj is Window w && w.Handle == Handle;
        
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
        public static bool operator ==(Window W1, Window W2) => W1?.Handle == W2?.Handle;

        /// <summary>
        /// Checks whether two <see cref="Window"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Window W1, Window W2) => !(W1 == W2);
    }
}