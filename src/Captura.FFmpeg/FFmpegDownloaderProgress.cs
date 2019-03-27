namespace Captura.FFmpeg
{
    public class FFmpegDownloaderProgress
    {
        public FFmpegDownloaderProgress(int Progress)
        {
            State = FFmpegDownloaderState.Downloading;
            DownloadProgress = Progress;
        }

        public FFmpegDownloaderProgress(string ErrorMessage)
        {
            State = FFmpegDownloaderState.Error;
            this.ErrorMessage = ErrorMessage;
        }

        public FFmpegDownloaderProgress(FFmpegDownloaderState State)
        {
            this.State = State;
        }

        public FFmpegDownloaderState State { get; }

        public int DownloadProgress { get; }

        public string ErrorMessage { get; }
    }
}