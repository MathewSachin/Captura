using System.Windows;

namespace Captura
{
    public partial class SourcePicker
    {
        public SourcePicker()
        {
            InitializeComponent();
        }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            Close();
        }
    }
}
