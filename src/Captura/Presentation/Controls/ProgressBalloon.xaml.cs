using System.Windows;
using System.Windows.Input;
using System;

namespace Captura
{
    public partial class ProgressBalloon : IRemoveRequester
    {
        public TrayProgressViewModel ViewModel { get; }

        public ProgressBalloon(TrayProgressViewModel ViewModel)
        {
            this.ViewModel = ViewModel;

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
            if (DataContext is TrayProgressViewModel vm)
            {
                if (vm.OnClick == null)
                    return;

                vm.OnClick.Invoke();
            }
        }
    }
}