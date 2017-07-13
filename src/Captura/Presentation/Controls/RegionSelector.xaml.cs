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

            #region Height and Width Boxes
            WidthBox.Minimum = (int)MinWidth - WidthBorder;
            HeightBox.Minimum = (int)MinHeight - HeightBorder;

            void sizeChange()
            {
                var selectedRegion = SelectedRegion;

                WidthBox.Value = selectedRegion.Width;
                HeightBox.Value = selectedRegion.Height;
            }

            SizeChanged += (s, e) => sizeChange();

            sizeChange();
            
            WidthBox.ValueChanged += (s, e) =>
            {
                if (e.NewValue == null)
                    return;

                var selectedRegion = SelectedRegion;

                selectedRegion.Width = WidthBox.Value.Value;

                SelectedRegion = selectedRegion;
            };

            HeightBox.ValueChanged += (s, e) =>
            {
                if (e.NewValue == null)
                    return;

                var selectedRegion = SelectedRegion;

                selectedRegion.Height = HeightBox.Value.Value;

                SelectedRegion = selectedRegion;
            };
            #endregion
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

            try
            {
                if (win == IntPtr.Zero)
                    SelectWindow();

                if (win != IntPtr.Zero)
                    SelectedRegion = new Screna.Window(win).Rectangle;
            }
            finally
            {
                win = IntPtr.Zero;
            }
        }

        IntPtr win;
        
        void SelectWindow()
        {
            var oldwin = win;

            win = WindowFromPoint(new Point((int)(Left - 1), (int)Top - 1) * Dpi.Instance);

            if (oldwin != IntPtr.Zero && oldwin != win)
            {
                ToggleBorder(oldwin);
                ToggleBorder(win);
            }
        }

        void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_captured)
            {
                SelectWindow();
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

        const int LeftOffset = 7,
            TopOffset = 37,
            WidthBorder = 14,
            HeightBorder = 74;

        // Ignoring Borders and Header
        public Rectangle SelectedRegion
        {
            get => Dispatcher.Invoke(() => new Rectangle((int)Left + LeftOffset, (int)Top + TopOffset, (int)Width - WidthBorder, (int)Height - HeightBorder)) * Dpi.Instance;
            set
            {
                if (value == Rectangle.Empty)
                    return;

                // High Dpi fix
                value *= Dpi.Inverse;

                Dispatcher.Invoke(() =>
                {
                    Left = value.Left - LeftOffset;
                    Top = value.Top - TopOffset;

                    Width = value.Width + WidthBorder;
                    Height = value.Height + HeightBorder;

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

                WidthBox.IsEnabled = HeightBox.IsEnabled = false;
            });
        }
        
        public void Release()
        {
            Dispatcher.Invoke(() =>
            {
                ResizeMode = ResizeMode.CanResize;
                Snapper.IsEnabled = CloseButton.IsEnabled = true;

                WidthBox.IsEnabled = HeightBox.IsEnabled = true;
            });
        }

        public IVideoItem VideoSource { get; }
        #endregion
    }
}
