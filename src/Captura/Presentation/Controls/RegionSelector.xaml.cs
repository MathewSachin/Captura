using Captura.Models;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace Captura
{
    public partial class RegionSelector : IRegionProvider
    {
        #region Native
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point Point);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        
        [DllImport("gdi32.dll")]
        static extern bool PatBlt(IntPtr hDC, int X, int Y, int Width, int Height, uint dwRop);
        #endregion

        const int DstInvert = 0x0055_0009;

        static void ToggleBorder(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return;

            var hdc = GetWindowDC(hWnd);

            var rect = new Screna.Window(hWnd).Rectangle;

            var borderThickness = Settings.Instance.RegionBorderThickness;

            // Top
            PatBlt(hdc, 0, 0, rect.Width, borderThickness, DstInvert);

            // Left
            PatBlt(hdc, 0, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Right
            PatBlt(hdc, rect.Width - borderThickness, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Bottom
            PatBlt(hdc, 0, rect.Height - borderThickness, rect.Width, borderThickness, DstInvert);
        }
        
        public RegionSelector()
        {
            InitializeComponent();

            VideoSource = new RegionItem(this);

            // Prevent being Maximized
            StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
            };

            // Prevent Closing by User
            Closing += (s, e) => e.Cancel = true;
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            SelectorHidden?.Invoke();
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

            try { SelectedRegion = new Screna.Window(win).Rectangle; }
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
                
                win = WindowFromPoint(new Point((int)(Left - 1), (int)Top - 1) * Dpi.Instance);

                if (oldwin != win)
                {
                    ToggleBorder(oldwin);
                    ToggleBorder(win);
                }
            }
        }

        #region IRegionProvider
        public event Action SelectorHidden;

        public bool SelectorVisible
        {
            get => Visibility == Visibility.Visible;
            set
            {
                if (value)
                    Show();
                else Hide();
            }
        }

        // Ignoring Borders and Header
        public Rectangle SelectedRegion
        {
            get => Dispatcher.Invoke(() => new Rectangle((int)Left + 7, (int)Top + 37, (int)Width - 14, (int)Height - 44)) * Dpi.Instance;
            set
            {
                if (value == Rectangle.Empty)
                    return;

                // High Dpi fix
                value *= Dpi.Inverse;

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

        public void Lock()
        {
            Dispatcher.Invoke(() =>
            {
                ResizeMode = ResizeMode.NoResize;
                Snapper.IsEnabled = CloseButton.IsEnabled = false;
            });
        }
        
        public void Release()
        {
            Dispatcher.Invoke(() =>
            {
                ResizeMode = ResizeMode.CanResize;
                Snapper.IsEnabled = CloseButton.IsEnabled = true;
            });
        }

        public IVideoItem VideoSource { get; }
        #endregion
    }
}
