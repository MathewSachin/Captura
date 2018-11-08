using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Captura
{
    public partial class PreviewWindow
    {
        bool _fullScreen;
        WindowState _lastState;

        static readonly TimeSpan TimeoutToHide = TimeSpan.FromSeconds(5);
        DateTime _lastMouseMoveTime;
        bool _hidden;
        readonly DispatcherTimer _timer;

        public PreviewWindow()
        {
            InitializeComponent();

            StrectValues.ItemsSource = new[]
            {
                Stretch.Uniform,
                Stretch.Fill,
                Stretch.UniformToFill
            };

            StrectValues.SelectedIndex = 0;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += TimerOnTick;

            _timer.Start();
        }

        void TimerOnTick(object Sender, EventArgs Args)
        {
            var elapsed = DateTime.Now - _lastMouseMoveTime;

            if (elapsed >= TimeoutToHide && !_hidden)
            {
                Cursor = Cursors.None;
                Toggler.BeginAnimation(OpacityProperty, new DoubleAnimation(Toggler.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(200))));
                _hidden = true;
            }
        }

        void StrectValues_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (StrectValues.SelectedValue is Stretch stretch)
            {
                DisplayImage.Stretch = stretch;
            }
        }

        void Fullscreen_OnClick(object Sender, RoutedEventArgs E)
        {
            ToggleFullscreen();
        }

        void ToggleFullscreen()
        {
            var icons = ServiceProvider.Get<IIconSet>();

            if (_fullScreen)
            {
                ConfigPanel.Visibility = Visibility.Visible;

                WindowStyle = WindowStyle.SingleBorderWindow;

                WindowState = _lastState;

                ResizeMode = ResizeMode.CanResize;

                ToggleButton.IconData = Geometry.Parse(icons.Expand);
            }
            else
            {
                ConfigPanel.Visibility = Visibility.Collapsed;

                WindowStyle = WindowStyle.None;

                _lastState = WindowState;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;

                ResizeMode = ResizeMode.NoResize;

                ToggleButton.IconData = Geometry.Parse(icons.Collapse);
            }

            _fullScreen = !_fullScreen;
        }

        void Zoom_OnExecuted(object Sender, ExecutedRoutedEventArgs E)
        {
            ToggleFullscreen();
        }

        void DecreaseZoom_OnExecuted(object Sender, ExecutedRoutedEventArgs E)
        {
            if (_fullScreen)
            {
                ToggleFullscreen();
            }
        }

        void PreviewWindow_OnMouseDoubleClick(object Sender, MouseButtonEventArgs E)
        {
            ToggleFullscreen();
        }

        void PreviewWindow_OnMouseMove(object Sender, MouseEventArgs E)
        {
            _lastMouseMoveTime = DateTime.Now;

            if (_hidden)
            {
                Cursor = Cursors.Arrow;
                Toggler.BeginAnimation(OpacityProperty, new DoubleAnimation(Toggler.Opacity, 0.8, new Duration(TimeSpan.FromMilliseconds(200))));
                _hidden = false;
            }
        }

        void PreviewWindow_OnIsVisibleChanged(object Sender, DependencyPropertyChangedEventArgs E)
        {
            if (Visibility == Visibility.Visible && !_timer.IsEnabled)
            {
                _timer.Start();
            }
            else if (Visibility != Visibility.Visible && _timer.IsEnabled)
            {
                _timer.Stop();
            }
        }
    }
}
