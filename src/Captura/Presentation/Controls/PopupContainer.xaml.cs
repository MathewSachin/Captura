using System.Windows;

namespace Captura
{
    public partial class PopupContainer
    {
        public PopupContainer()
        {
            InitializeComponent();
        }

        void OnClose()
        {
            Visibility = Visibility.Collapsed;

            ItemsControl.Items.Clear();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();

        void Remove(FrameworkElement Element)
        {
            ItemsControl.Items.Remove(Element);

            if (ItemsControl.Items.Count == 0)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public void Add(FrameworkElement Element)
        {
            if (Element is ScreenShotBalloon screenShotBalloon)
            {
                screenShotBalloon.RemoveRequested += () => Remove(screenShotBalloon);
            }

            if (Element is TextBalloon textBalloon)
            {
                textBalloon.RemoveRequested += () => Remove(textBalloon);
            }

            ItemsControl.Items.Insert(0, Element);
        }
    }
}