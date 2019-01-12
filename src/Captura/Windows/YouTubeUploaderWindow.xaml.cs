using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class YouTubeUploaderWindow
    {
        public YouTubeUploaderWindow()
        {
            InitializeComponent();

            Closing += async (S, E) =>
            {
                if (DataContext is YouTubeUploaderViewModel vm)
                {
                    if (!await vm.Cancel())
                    {
                        E.Cancel = true;
                    }
                }
            };
        }

        public async void Open(string FileName)
        {
            if (DataContext is YouTubeUploaderViewModel vm)
            {
                await vm.Init(FileName);
            }
        }

        void Cancel_Click(object Sender, RoutedEventArgs E)
        {
            Close();
        }
    }
}
