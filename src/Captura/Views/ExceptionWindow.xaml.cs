using System;
using System.Windows;

namespace Captura.Views
{
    public partial class ExceptionWindow
    {
        public ExceptionWindow(Exception Exception)
        {
            InitializeComponent();

            if (DataContext is ExceptionViewModel vm)
            {
                vm.Init(Exception);
            }
        }

        void Close_OnClick(object Sender, RoutedEventArgs E)
        {
            Close();
        }
    }
}
