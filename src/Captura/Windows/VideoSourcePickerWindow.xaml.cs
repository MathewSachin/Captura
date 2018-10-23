﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Captura.Models;
using Screna;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
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

        readonly VideoPickerMode _mode;

        List<IntPtr> SkipWindows { get; } = new List<IntPtr>();

        VideoSourcePickerWindow(VideoPickerMode Mode)
        {
            _mode = Mode;
            InitializeComponent();

            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
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

        void BeginClose()
        {
            var duration = new Duration(TimeSpan.FromMilliseconds(200));

            var opacityAnim = new DoubleAnimation(0, duration);

            opacityAnim.Completed += (S, E) => Close();

            BeginAnimation(OpacityProperty, opacityAnim);
        }

        void ShowCancelText()
        {
            foreach (var screen in _screens)
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
                    Text = $"Select {_mode} or Press Esc to Cancel",
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
            SelectedScreen = null;
            SelectedWindow = null;

            BeginClose();
        }

        Rectangle? _lastRectangle;

        void UpdateBorderAndCursor(Rectangle? Rect)
        {
            if (_lastRectangle == Rect)
                return;

            _lastRectangle = Rect;

            var storyboard = new Storyboard();

            var duration = new Duration(TimeSpan.FromMilliseconds(100));

            if (Rect == null)
            {
                Cursor = Cursors.Arrow;

                var opacityAnim = new DoubleAnimation(0, duration);
                Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(nameof(Opacity)));
                storyboard.Children.Add(opacityAnim);
            }
            else
            {
                Cursor = Cursors.Hand;

                var opacityAnim = new DoubleAnimation(1, duration);
                Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(nameof(Opacity)));
                storyboard.Children.Add(opacityAnim);

                var rect = Rect.Value;

                var margin = new Thickness(-Left + rect.Left / Dpi.X, -Top + rect.Top / Dpi.Y, 0, 0);
                var marginAnim = new ThicknessAnimation(margin, duration);
                Storyboard.SetTargetProperty(marginAnim, new PropertyPath(nameof(Margin)));
                storyboard.Children.Add(marginAnim);

                var widthAnim = new DoubleAnimation(Border.ActualWidth, rect.Width / Dpi.X, duration);
                Storyboard.SetTargetProperty(widthAnim, new PropertyPath(nameof(Width)));
                storyboard.Children.Add(widthAnim);

                var heightAnim = new DoubleAnimation(Border.ActualHeight, rect.Height / Dpi.Y, duration);
                Storyboard.SetTargetProperty(heightAnim, new PropertyPath(nameof(Height)));
                storyboard.Children.Add(heightAnim);
            }

            Border.BeginStoryboard(storyboard);
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
            var point = MouseCursor.CursorPosition;

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
                    BeginClose();
                    break;
            }
        }

        public static IScreen PickScreen()
        {
            var picker = new VideoSourcePickerWindow(VideoPickerMode.Screen);

            picker.ShowDialog();

            return picker.SelectedScreen == null ? null : new ScreenWrapper(picker.SelectedScreen);
        }

        public static IWindow PickWindow(IEnumerable<IntPtr> SkipWindows)
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
