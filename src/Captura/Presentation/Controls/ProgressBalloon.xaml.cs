using System.Windows;
using System.Windows.Input;
using System;

namespace Captura
{
    public partial class ProgressBalloon : IRemoveRequester, ITrayProgress
    {
        Action _onClick;
        
        public ProgressBalloon()
        {
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
            if (_onClick == null)
                return;

            _onClick.Invoke();

            OnClose();
        }

        public void RegisterClick(Action OnClick)
        {
            _onClick = OnClick;
        }

        public void UpdateProgress(int Progress)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrimaryText(string Text)
        {
            throw new NotImplementedException();
        }

        public void UpdateSecondaryText(string Text)
        {
            throw new NotImplementedException();
        }

        public void Finish(bool Success)
        {
            throw new NotImplementedException();
        }
    }
}