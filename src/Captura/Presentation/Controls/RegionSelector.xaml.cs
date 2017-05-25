using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace Captura
{
    public partial class RegionSelector
    {
        #region Native
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point Point);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        
        [DllImport("gdi32.dll")]
        static extern bool PatBlt(IntPtr hDC, int X, int Y, int Width, int Height, uint dwRop);
        #endregion

        const int DstInvert = 0x0055_0009,
            borderThickness = 3;

        static void ToggleBorder(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return;

            var hdc = GetWindowDC(hWnd);

            var rect = new Screna.Window(hWnd).Rectangle;
            
            // Top
            PatBlt(hdc, 0, 0, rect.Width, borderThickness, DstInvert);

            // Left
            PatBlt(hdc, 0, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Right
            PatBlt(hdc, rect.Width - borderThickness, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Bottom
            PatBlt(hdc, 0, rect.Height - borderThickness, rect.Width, borderThickness, DstInvert);
        }

        public static RegionSelector Instance { get; } = new RegionSelector();

        RegionSelector()
        {
            InitializeComponent();

            // Prevent being Maximized
            StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
            };

            // Prevent Closing by User
            Closing += (s, e) => e.Cancel = true;
        }

        // Ignoring Borders and Header
        public Rectangle Rectangle
        {
            get => Dispatcher.Invoke(() => new Rectangle((int) Left + 7, (int) Top + 37, (int) Width - 14, (int) Height - 44));
            set
            {
                if (value == Rectangle.Empty)
                    return;

                Dispatcher.Invoke(() =>
                {
                    Left = value.Left - 7;
                    Top = value.Top - 37;

                    Width = value.Width + 14;
                    Height = value.Height + 44;

                    // Prevent going off-screen
                    if (Left < 0)
                    {
                        // Decrease Width
                        Width += Left;
                        Left = 0;
                    }

                    if (Top < 0)
                    {
                        // Decrease Height
                        Height += Top;
                        Top = 0;
                    }
                });
            }
        }

        public bool SnapEnabled
        {
            get => Dispatcher.Invoke(() => Snapper.IsEnabled);
            set => Dispatcher.Invoke(() => Snapper.IsEnabled = value);
        }

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        bool _captured;

        void ModernButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _captured = true;

            DragMove();

            _captured = false;

            ToggleBorder(win);

            try { Rectangle = new Screna.Window(win).Rectangle; }
            finally
            {
                win = IntPtr.Zero;
            }
        }

        IntPtr win;

        void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_captured)
            {
                var oldwin = win;
                
                win = WindowFromPoint(new Point((int)Left - 1, (int)Top - 1));

                if (oldwin != win)
                {
                    ToggleBorder(oldwin);
                    ToggleBorder(win);
                }
            }
        }
    }
}
