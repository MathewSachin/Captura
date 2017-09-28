using Captura.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura.Models
{
    public class MessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = Resources.ErrorOccured,
                    Content = Message
                };

                dialog.OkButton.Content = Resources.Ok;
                dialog.Buttons = new[] { dialog.OkButton };

                dialog.BackgroundContent = new Grid
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 10,
                };

                dialog.ShowDialog();
            });
        }

        public void ShowFFMpegUnavailable()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = "FFMpeg Unavailable",
                    Content = "FFMpeg was not found on your system.\n\nSelect FFMpeg Folder if you alrady have FFMpeg on your system, else Download FFMpeg."
                };

                // Yes -> Select FFMpeg Folder
                dialog.YesButton.Content = Resources.SelectFFMpegFolder;
                dialog.YesButton.Click += (s, e) => FFMpegService.SelectFFMpegFolder();

                // No -> Download FFMpeg
                dialog.NoButton.Content = "Download FFMpeg";
                dialog.NoButton.Click += (s, e) => FFMpegService.FFMpegDownloader?.Invoke();

                dialog.CancelButton.Content = "Cancel";

                dialog.Buttons = new[] { dialog.YesButton, dialog.NoButton, dialog.CancelButton };

                dialog.ShowDialog();
            });
        }

        public bool ShowYesNo(string Message, string Title)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = Title,
                    Content = Message
                };

                var result = false;

                dialog.YesButton.Content = Resources.Yes;
                dialog.YesButton.Click += (s, e) => result = true;

                dialog.NoButton.Content = Resources.No;

                dialog.Buttons = new[] { dialog.YesButton, dialog.NoButton };

                dialog.ShowDialog();

                return result;
            });
        }
    }
}
