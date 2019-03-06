namespace Captura.FFmpeg
{
    public interface IFFmpegViewsProvider
    {
        void ShowLogs();

        void ShowUnavailable();

        void ShowDownloader();

        void PickFolder();
    }
}