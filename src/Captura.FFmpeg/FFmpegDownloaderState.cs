namespace Captura.FFmpeg
{
    public enum FFmpegDownloaderState
    {
        Ready,
        Downloading,
        Extracting,
        Done,
        Cancelled,
        Error
    }
}