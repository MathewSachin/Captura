using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace Captura
{
    public partial class RegionPickerWindow
    {
        RegionPickerWindow()
        {
            InitializeComponent();

            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            ShowCancelText();
        }

        void ShowCancelText()
        {
            foreach (var screen in Screen.AllScreens)
            {
                var left = -Left + screen.Bounds.Left / Dpi.X;
                var top = -Top + screen.Bounds.Top / Dpi.Y;
                var width = screen.Bounds.Width / Dpi.X;
                var height = screen.Bounds.Height / Dpi.Y;

                var container = new ContentControl
                {
                    Width = width,
                    Height = height,
                    Margin = new Thickness(left, top, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                var textBlock = new TextBlock
                {
                    Text = $"Drag to Select Region or Press Esc to Cancel",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(10, 5, 10, 5),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Color.FromArgb(183, 0, 0, 0))
                };

                container.Content = textBlock;

                Grid.Children.Add(container);
            }
        }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            _start = _end = null;

            Close();
        }

        void UpdateSizeDisplay(Rect? Rect)
        {
            if (Rect == null)
            {
                SizeText.Visibility = Visibility.Collapsed;
            }
            else
            {
                SizeText.Text = $"{(int) Rect.Value.Width} x {(int) Rect.Value.Height}";

                SizeText.Margin = new Thickness(30, 20, 0, 0);

                SizeText.Visibility = Visibility.Visible;
            }
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
            if (_isDragging)
            {
                _end = E.GetPosition(Grid);

                var r = GetRegion();

                UpdateSizeDisplay(r);

                if (r == null)
                {
                    Border.Visibility = Visibility.Collapsed;
                    return;
                }

                var rect = r.Value;

                Border.Margin = new Thickness(rect.Left, rect.Top, 0, 0);

                Border.Width = rect.Width;
                Border.Height = rect.Height;

                Border.Visibility = Visibility.Visible;
            }
        }

        bool _isDragging;
        Point? _start, _end;
        CroppingAdorner _croppingAdorner;

        void WindowMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            _isDragging = true;
            _start = E.GetPosition(Grid);
            _end = null;

            if (_croppingAdorner != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(Grid);

                layer.Remove(_croppingAdorner);

                _croppingAdorner = null;
            }
        }

        void WindowMouseLeftButtonUp(object Sender, MouseButtonEventArgs E)
        {
            _isDragging = false;
            _end = E.GetPosition(Grid);
            Border.Visibility = Visibility.Collapsed;

            var layer = AdornerLayer.GetAdornerLayer(Grid);

            var rect = GetRegion();

            UpdateSizeDisplay(rect);

            if (rect == null)
                return;

            _croppingAdorner = new CroppingAdorner(Grid, rect.Value);

            var clr = Colors.Black;
            clr.A = 110;
            _croppingAdorner.Fill = new SolidColorBrush(clr);

            layer.Add(_croppingAdorner);

            _croppingAdorner.CropChanged += (S, Args) => UpdateSizeDisplay(_croppingAdorner.SelectedRegion);

            _croppingAdorner.Checked += () =>
            {
                var r = _croppingAdorner.SelectedRegion;

                _start = r.Location;
                _end = r.BottomRight;

                Close();
            };
        }

        Rect? GetRegion()
        {
            if (_start == null || _end == null)
            {
                return null;
            }

            var end = _end.Value;
            var start = _start.Value;

            if (end.X < start.X)
            {
                var t = start.X;
                start.X = end.X;
                end.X = t;
            }

            if (end.Y < start.Y)
            {
                var t = start.Y;
                start.Y = end.Y;
                end.Y = t;
            }

            var width = end.X - start.X;
            var height = end.Y - start.Y;

            if (width < 0.01 || height < 0.01)
            {
                return null;
            }

            return new Rect(start.X, start.Y, width, height);
        }

        Rectangle? GetRegionScaled()
        {
            var rect = GetRegion();

            if (rect == null)
            {
                return null;
            }

            var r = rect.Value;

            return new Rectangle((int) ((Left + r.X) * Dpi.X),
                (int)((Top + r.Y) * Dpi.Y),
                (int)(r.Width * Dpi.X),
                (int)(r.Height * Dpi.Y));
        }

        public static Rectangle? PickRegion()
        {
            var picker = new RegionPickerWindow();

            picker.ShowDialog();

            return picker.GetRegionScaled();
        }
    }
}
