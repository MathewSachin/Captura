using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class YouTubeUploaderWindow
    {
        public YouTubeUploaderWindow()
        {
            InitializeComponent();
        }

        public void Open(string FileName)
        {
            if (DataContext is YouTubeUploaderViewModel vm)
            {
                vm.FileName = FileName;
            }
        }

        void Cancel_Click(object Sender, RoutedEventArgs E)
        {
            Close();
        }
    }
}
