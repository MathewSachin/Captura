using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using Captura.Models;

namespace Captura
{
    public partial class NotificationStack
    {
        public NotificationStack()
        {
            InitializeComponent();
        }

        void Hide()
        {
            ServiceProvider.Get<ISystemTray>().HideNotification();
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

        void Remove(FrameworkElement Element)
        {
            var anim = new DoubleAnimation(Element.ActualHeight, 0, new Duration(TimeSpan.FromMilliseconds(200)));

            anim.Completed += (S, E) =>
            {
                ItemsControl.Items.Remove(Element);

                if (ItemsControl.Items.Count == 0)
                {
                    Hide();
                }
            };

            Element.BeginAnimation(HeightProperty, anim);
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
                    if (frameworkElement is ProgressBalloon progressBalloon && !progressBalloon.ViewModel.Finished)
                        continue;

                    Remove(frameworkElement);
                }
            }
        }
    }
}