using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using Captura.Models;

namespace Captura
{
    public partial class PopupContainer
    {
        public PopupContainer()
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
            switch (Element)
            {
                case ScreenShotBalloon screenShotBalloon:
                    screenShotBalloon.RemoveRequested += () => Remove(screenShotBalloon);
                    break;

                case TextBalloon textBalloon:
                    textBalloon.RemoveRequested += () => Remove(textBalloon);
                    break;

                case StatusBalloon statusBalloon:
                    statusBalloon.RemoveRequested += () => Remove(statusBalloon);
                    break;
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
                    Remove(frameworkElement);
                }
            }
        }
    }
}