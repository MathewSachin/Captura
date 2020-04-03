using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Captura.Audio;
using Captura.Loc;
using Captura.Views;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MessageProvider : IMessageProvider
    {
        readonly IAudioPlayer _audioPlayer;
        readonly ILocalizationProvider _loc;

        public MessageProvider(IAudioPlayer AudioPlayer, ILocalizationProvider Loc)
        {
            _audioPlayer = AudioPlayer;
            _loc = Loc;
        }

        public void ShowError(string Message, string Header = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = _loc.ErrorOccurred,
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

                dialog.OkButton.Content = _loc.Ok;
                dialog.Buttons = new[] { dialog.OkButton };

                dialog.BackgroundContent = new Grid
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 10
                };

                _audioPlayer.Play(SoundKind.Error);

                dialog.ShowDialog();
            });
        }

        public void ShowException(Exception Exception, string Message, bool Blocking = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var win = new ErrorWindow(Exception, Message);

                _audioPlayer.Play(SoundKind.Error);

                if (Blocking)
                {
                    win.ShowDialog();
                }
                else win.ShowAndFocus();
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

                dialog.YesButton.Content = _loc.Yes;
                dialog.YesButton.Click += (S, E) => result = true;

                dialog.NoButton.Content = _loc.No;

                dialog.Buttons = new[] { dialog.YesButton, dialog.NoButton };

                dialog.ShowDialog();

                return result;
            });
        }
    }
}
