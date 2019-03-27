using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Captura
{
    public partial class NotificationStack
    {
        static readonly TimeSpan TimeoutToHide = TimeSpan.FromSeconds(5);
        DateTime _lastMouseMoveTime;
        readonly DispatcherTimer _timer;

        public NotificationStack()
        {
            InitializeComponent();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += TimerOnTick;

            _timer.Start();
        }

        void TimerOnTick(object Sender, EventArgs Args)
        {
            var now = DateTime.Now;
            var elapsed = now - _lastMouseMoveTime;

            var unfinished = ItemsControl.Items
                .OfType<NotificationBalloon>()
                .Any(M => !M.Notification.Finished);

            if (unfinished)
            {
                _lastMouseMoveTime = now;
            }

            if (elapsed < TimeoutToHide)
                return;

            if (!unfinished)
            {
                OnClose();
            }
        }

        public void Hide()
        {
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(100))));

            if (_timer.IsEnabled)
                _timer.Stop();
        }

        public void Show()
        {
            _lastMouseMoveTime = DateTime.Now;

            BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(300))));

            if (!_timer.IsEnabled)
                _timer.Start();
        }

        void OnClose()
        {
            Hide();

            var copy = ItemsControl.Items.OfType<FrameworkElement>().ToArray();

            foreach (var frameworkElement in copy)
            {
                Remove(frameworkElement);
            }
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();

        /// <summary>
        /// Slides out element while decreasing opacity, then decreases height, then removes.
        /// </summary>
        void Remove(FrameworkElement Element)
        {
            var transform = new TranslateTransform();
            Element.RenderTransform = transform;

            var translateAnim = new DoubleAnimation(500, new Duration(TimeSpan.FromMilliseconds(200)));
            var opactityAnim = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));

            var heightAnim = new DoubleAnimation(Element.ActualHeight, 0, new Duration(TimeSpan.FromMilliseconds(200)));

            heightAnim.Completed += (S, E) =>
            {
                ItemsControl.Items.Remove(Element);

                if (ItemsControl.Items.Count == 0)
                {
                    Hide();
                }
            };

            opactityAnim.Completed += (S, E) => Element.BeginAnimation(HeightProperty, heightAnim);

            transform.BeginAnimation(TranslateTransform.XProperty, translateAnim);
            Element.BeginAnimation(OpacityProperty, opactityAnim);
        }

        const int MaxItems = 5;

        public void Add(FrameworkElement Element)
        {
            if (Element is IRemoveRequester removeRequester)
            {
                removeRequester.RemoveRequested += () => Remove(Element);
            }

            if (Element is ScreenShotBalloon ssBalloon)
                ssBalloon.Expander.IsExpanded = true;

            foreach (var item in ItemsControl.Items)
            {
                if (item is ScreenShotBalloon screenShotBalloon)
                {
                    screenShotBalloon.Expander.IsExpanded = false;
                }
            }

            ItemsControl.Items.Insert(0, Element);

            if (ItemsControl.Items.Count > MaxItems)
            {
                var itemsToRemove = ItemsControl.Items
                    .OfType<FrameworkElement>()
                    .Skip(MaxItems)
                    .ToArray();

                foreach (var frameworkElement in itemsToRemove)
                {
                    if (frameworkElement is NotificationBalloon progressBalloon && !progressBalloon.Notification.Finished)
                        continue;

                    Remove(frameworkElement);
                }
            }
        }

        void NotificationStack_OnMouseMove(object Sender, MouseEventArgs E)
        {
            if (ItemsControl.Items.Count == 0)
                return;

            _lastMouseMoveTime = DateTime.Now;

            Show();
        }
    }
}