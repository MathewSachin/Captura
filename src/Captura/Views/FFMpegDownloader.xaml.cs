using System.Windows;
using System.Windows.Shell;
using Captura.ViewModels;

namespace Captura.Views
{
    public partial class FFmpegDownloader
    {
        public FFmpegDownloader()
        {
            DataContextChanged += (S, E) =>
            {
                if (DataContext is FFmpegDownloadViewModel vm)
                {
                    vm.CloseWindowAction += Close;

                    vm.ProgressChanged += P =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                            TaskbarItemInfo.ProgressValue = P / 100.0;
                        });
                    };

                    vm.AfterDownload += Success =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TaskbarItemInfo.ProgressState = Success ? TaskbarItemProgressState.None : TaskbarItemProgressState.Error;
                            TaskbarItemInfo.ProgressValue = 1;
                        });
                    };
                }
            };

            InitializeComponent();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
    }
}
