using System.Windows;
using System;
using System.Windows.Media;

namespace Captura
{
    public partial class StatusBalloon : IRemoveRequester
    {
        public StatusBalloon(string Text, bool Error)
        {
            DataContext = Text;

            InitializeComponent();

            if (Error)
                TextBlock.Foreground = Brushes.LightPink;
        }

        void OnClose()
        {
            RemoveRequested?.Invoke();
        }

        public event Action RemoveRequested;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();
    }
}