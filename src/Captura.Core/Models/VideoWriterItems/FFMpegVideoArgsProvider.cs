namespace Captura.Models
{
    /// <summary>
    /// Provides Command-line args for FFmpeg Video encoding.
    /// </summary>
    /// <param name="VideoQuality">Video Quality... 1 to 100.</param>
    public delegate string FFmpegVideoArgsProvider(int VideoQuality);
}