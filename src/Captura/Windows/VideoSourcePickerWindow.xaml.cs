using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Captura.Models;
using Screna;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;
using Window = Screna.Window;

namespace Captura
{
    public partial class VideoSourcePickerWindow
    {
        enum VideoPickerMode
        {
            Window,
            Screen
        }

        VideoPickerMode _mode;

        public List<IntPtr> SkipWindows { get; } = new List<IntPtr>();

        VideoSourcePickerWindow(VideoPickerMode Mode)
        {
            _mode = Mode;
            InitializeComponent();

            Left = Top = 0;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            UpdateBackground();

            _screens = Screen.AllScreens;
            _windows = Window.EnumerateVisible().ToArray();

            ShowCancelText();
        }

        readonly Screen[] _screens;

        readonly Window[] _windows;

        public Screen SelectedScreen { get; private set; }

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
            foreach (var screen in _screens)
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
                    Text = $"Select {_mode} or Press Esc to Cancel",
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
            SelectedScreen = null;
            SelectedWindow = null;

            Close();
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
            var pos = E.GetPosition(this);

            var point = new Point((int) (pos.X * Dpi.X), (int) (pos.Y * Dpi.Y));

            void UpdateBorderAndCursor(Rectangle? Rect)
            {
                if (Rect == null)
                {
                    Cursor = Cursors.Arrow;

                    Border.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Cursor = Cursors.Hand;

                    var rect = Rect.Value;

                    Border.Margin = new Thickness(rect.Left / Dpi.X, rect.Top / Dpi.Y, 0, 0);

                    Border.Width = rect.Width / Dpi.X;
                    Border.Height = rect.Height / Dpi.Y;

                    Border.Visibility = Visibility.Visible;
                }
            }

            switch (_mode)
            {
                case VideoPickerMode.Screen:
                    SelectedScreen = _screens.FirstOrDefault(M => M.Bounds.Contains(point));

                    UpdateBorderAndCursor(SelectedScreen?.Bounds);
                    break;

                case VideoPickerMode.Window:
                    SelectedWindow = _windows
                        .Where(M => !SkipWindows.Contains(M.Handle))
                        .FirstOrDefault(M => M.Rectangle.Contains(point));
                    
                    UpdateBorderAndCursor(SelectedWindow?.Rectangle);
                    break;
            }
        }

        void WindowMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            switch (_mode)
            {
                case VideoPickerMode.Screen when SelectedScreen != null:
                case VideoPickerMode.Window when SelectedWindow != null:
                    Close();
                    break;
            }
        }

        public static IScreen PickScreen()
        {
            var picker = new VideoSourcePickerWindow(VideoPickerMode.Screen);

            picker.ShowDialog();

            return new ScreenWrapper(picker.SelectedScreen);
        }

        public static Window PickWindow(IEnumerable<IntPtr> SkipWindows)
        {
            var picker = new VideoSourcePickerWindow(VideoPickerMode.Window)
            {
                Border =
                {
                    BorderThickness = new Thickness(5)
                }
            };

            if (SkipWindows != null)
            {
                picker.SkipWindows.AddRange(SkipWindows);
            }

            picker.ShowDialog();

            return picker.SelectedWindow;
        }
    }
}
