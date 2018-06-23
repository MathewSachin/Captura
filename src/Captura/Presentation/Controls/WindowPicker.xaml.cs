using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Screna;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;
using Window = Screna.Window;

namespace Captura
{
    public partial class WindowPicker
    {
        readonly IEnumerable<IntPtr> _skipWindows;

        public WindowPicker(IEnumerable<IntPtr> SkipWindows)
        {
            _skipWindows = SkipWindows ?? new IntPtr[0];

            InitializeComponent();

            Left = Top = 0;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            UpdateBackground();

            ShowCancelText();

            _windows = Window.EnumerateVisible().ToArray();
        }

        readonly Window[] _windows;

        public Window SelectedWindow { get; private set; }

        void UpdateBackground()
        {
            using (var bmp = ScreenShot.Capture())
            {
                var stream = new MemoryStream();
                bmp.Save(stream, ImageFormat.Png);

                stream.Seek(0, SeekOrigin.Begin);

                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                Background = new ImageBrush(decoder.Frames[0]);
            }
        }

        void ShowCancelText()
        {
            foreach (var screen in Screen.AllScreens)
            {
                var left = screen.Bounds.Left / Dpi.X;
                var top = screen.Bounds.Top / Dpi.Y;
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
                    Text = "Select Window or Press Esc to Cancel",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                container.Content = textBlock;

                Grid.Children.Add(container);
            }
        }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            SelectedWindow = null;

            Close();
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
            var pos = E.GetPosition(this);

            var point = new Point((int) (pos.X * Dpi.X), (int) (pos.Y * Dpi.Y));

            var window = _windows
                .Where(M => !_skipWindows.Contains(M.Handle))
                .FirstOrDefault(M => M.Rectangle.Contains(point));

            if (window != null)
            {
                SelectedWindow = window;

                Cursor = Cursors.Hand;

                var rect = window.Rectangle;

                WindowBorder.Margin = new Thickness(rect.Left / Dpi.X, rect.Top / Dpi.Y, 0, 0);

                WindowBorder.Width = rect.Width / Dpi.X;
                WindowBorder.Height = rect.Height / Dpi.Y;
                
                WindowBorder.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedWindow = null;

                Cursor = Cursors.Arrow;

                WindowBorder.Visibility = Visibility.Collapsed;
            }
        }

        void WindowMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            if (SelectedWindow != null)
            {
                Close();
            }
        }
    }
}
