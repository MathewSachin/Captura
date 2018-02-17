using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura.Models
{
    public class MessageProvider : IMessageProvider
    {
        public void ShowError(string Message, string Header = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = LanguageManager.Instance.ErrorOccured,
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = Header,
                                Margin = new Thickness(0, 0, 0, 10),
                                FontSize = 15
                            },

                            new ScrollViewer
                            {
                                Content = Message,
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                                Padding = new Thickness(0, 0, 0, 10)
                            }
                        }
                    }
                };

                dialog.OkButton.Content = LanguageManager.Instance.Ok;
                dialog.Buttons = new[] { dialog.OkButton };

                dialog.BackgroundContent = new Grid
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 10
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
                dialog.YesButton.Content = LanguageManager.Instance.SelectFFMpegFolder;
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
                    Content = new ScrollViewer
                    {
                        Content = Message,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Padding = new Thickness(0, 0, 0, 10)
                    }
                };

                var result = false;

                dialog.YesButton.Content = LanguageManager.Instance.Yes;
                dialog.YesButton.Click += (s, e) => result = true;

                dialog.NoButton.Content = LanguageManager.Instance.No;

                dialog.Buttons = new[] { dialog.YesButton, dialog.NoButton };

                dialog.ShowDialog();

                return result;
            });
        }
    }
}
