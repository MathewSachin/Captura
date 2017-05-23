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
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point Point);
        
        public static RegionSelector Instance { get; } = new RegionSelector();

        RegionSelector()
        {
            InitializeComponent();

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

            try { Rectangle = new Screna.Window(win).Rectangle; }
            catch
            {
                // Suppress errors
            }
        }

        IntPtr win;

        void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_captured)
            {
                win = WindowFromPoint(new Point((int)Left - 1, (int)Top - 1));
            }
        }
    }
}
