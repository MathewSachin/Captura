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

        readonly Settings _settings;

        void ToggleBorder(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return;

            var hdc = GetWindowDC(hWnd);

            var rect = new Screna.Window(hWnd).Rectangle;

            var borderThickness = _settings.UI.RegionBorderThickness;

            // Top
            PatBlt(hdc, 0, 0, rect.Width, borderThickness, DstInvert);

            // Left
            PatBlt(hdc, 0, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Right
            PatBlt(hdc, rect.Width - borderThickness, borderThickness, borderThickness, rect.Height - 2 * borderThickness, DstInvert);

            // Bottom
            PatBlt(hdc, 0, rect.Height - borderThickness, rect.Width, borderThickness, DstInvert);
        }
        
        public RegionSelector(Settings Settings)
        {
            _settings = Settings;

            InitializeComponent();

            VideoSource = new RegionItem(this);

            // Prevent Closing by User
            Closing += (s, e) => e.Cancel = true;

            InitDimensionBoxes();
        }

        void InitDimensionBoxes()
        {
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
                if (e.NewValue is int width)
                {
                    var selectedRegion = SelectedRegion;

                    selectedRegion.Width = width;

                    SelectedRegion = selectedRegion;
                }
            };

            HeightBox.ValueChanged += (s, e) =>
            {
                if (e.NewValue is int height)
                {
                    var selectedRegion = SelectedRegion;

                    selectedRegion.Height = height;

                    SelectedRegion = selectedRegion;
                }
            };
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            SelectorHidden?.Invoke();
        }
        
        bool _captured;

        void SnapButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _captured = true;

            DragMove();

            _captured = false;
            
            try
            {
                if (win == IntPtr.Zero)
                {
                    win = WindowFromPoint(new Point((int)(Left - 1), (int)Top - 1) * Dpi.Instance);
                }
                else ToggleBorder(win);

                if (win != IntPtr.Zero)
                {
                    SelectedRegion = new Screna.Window(win).Rectangle;

                    // Prevent going outside
                    if (Left < 0)
                    {
                        // Decrease Width
                        try { Width += Left; }
                        catch { }
                        finally { Left = 0; }
                    }

                    if (Top < 0)
                    {
                        // Decrease Height
                        try { Height += Top; }
                        catch { }
                        finally { Top = 0; }
                    }
                }
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

            if (oldwin == IntPtr.Zero)
                ToggleBorder(win);
            else if (oldwin != win)
            {
                ToggleBorder(oldwin);
                ToggleBorder(win);
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            
            if (_captured)
            {
                SelectWindow();
            }

            UpdateRegion();
        }

        // Prevent Maximizing
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState != WindowState.Normal)
                WindowState = WindowState.Normal;

            base.OnStateChanged(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateRegion();

            base.OnRenderSizeChanged(sizeInfo);
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

        Rectangle? _region;
        
        void UpdateRegion()
        {
            _region = Dispatcher.Invoke(() => new Rectangle((int)Left + LeftOffset, (int)Top + TopOffset, (int)Width - WidthBorder, (int)Height - HeightBorder)) * Dpi.Instance;
        }

        // Ignoring Borders and Header
        public Rectangle SelectedRegion
        {
            get
            {
                if (_region == null)
                    UpdateRegion();

                return _region.Value;
            }
            set
            {
                if (value == Rectangle.Empty)
                    return;

                // High Dpi fix
                value *= Dpi.Inverse;

                Dispatcher.Invoke(() =>
                {
                    Width = value.Width + WidthBorder;
                    Height = value.Height + HeightBorder;

                    Left = value.Left - LeftOffset;
                    Top = value.Top - TopOffset;
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
