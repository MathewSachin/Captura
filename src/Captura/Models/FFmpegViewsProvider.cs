using System.Windows;
using Captura.Audio;
using Captura.Loc;
using Captura.Models;
using Captura.Views;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegViewsProvider : IFFmpegViewsProvider
    {
        readonly ILocalizationProvider _loc;
        readonly IAudioPlayer _audioPlayer;
        readonly FFmpegSettings _settings;
        readonly IDialogService _dialogService;

        public FFmpegViewsProvider(ILocalizationProvider Loc,
            IAudioPlayer AudioPlayer,
            FFmpegSettings Settings,
            IDialogService DialogService)
        {
            _loc = Loc;
            _audioPlayer = AudioPlayer;
            _settings = Settings;
            _dialogService = DialogService;
        }

        public void ShowLogs()
        {
            SettingsWindow.ShowFFmpegLogs();
        }

        public void ShowUnavailable()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ModernDialog
                {
                    Title = "FFmpeg Unavailable",
                    Content = "FFmpeg was not found on your system.\n\nSelect FFmpeg Folder if you alrady have FFmpeg on your system, else Download FFmpeg."
                };

                // Yes -> Select FFmpeg Folder
                dialog.YesButton.Content = _loc.SelectFFmpegFolder;
                dialog.YesButton.Click += (S, E) => PickFolder();

                // No -> Download FFmpeg
                dialog.NoButton.Content = _loc.DownloadFFmpeg;
                dialog.NoButton.Click += (S, E) => ShowDownloader();

                dialog.CancelButton.Content = "Cancel";

                dialog.Buttons = new[] { dialog.YesButton, dialog.NoButton, dialog.CancelButton };

                _audioPlayer.Play(SoundKind.Error);

                dialog.ShowDialog();
            });
        }

        public void ShowDownloader()
        {
            FFmpegDownloaderWindow.ShowInstance();
        }

        public void PickFolder()
        {
            var folder = _dialogService.PickFolder(_settings.GetFolderPath(), _loc.SelectFFmpegFolder);

            if (!string.IsNullOrWhiteSpace(folder))
                _settings.FolderPath = folder;
        }
    }
}