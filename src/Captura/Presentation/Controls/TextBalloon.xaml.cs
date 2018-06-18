using System.Windows;
using System.Windows.Input;
using System;

namespace Captura
{
    public partial class TextBalloon : IRemoveRequester
    {
        readonly Action _onClick;
        
        public TextBalloon(string Text, Action OnClick)
        {
            DataContext = Text;

            _onClick = OnClick;

            InitializeComponent();
        }

        void OnClose()
        {
            RemoveRequested?.Invoke();
        }

        public event Action RemoveRequested;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();
        
        void TextBlock_MouseUp(object Sender, MouseButtonEventArgs E)
        {
            _onClick?.Invoke();
        }
    }
}