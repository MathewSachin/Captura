namespace Captura.Models
{
    /// <summary>
    /// Provides FFmpeg Audio encoding Command-line args.
    /// </summary>
    /// <param name="AudioQuality">Audio Quality... 1 to 100.</param>
    /// <returns>FFmpeg Audio encoding Command-line args</returns>
    public delegate string FFmpegAudioArgsProvider(int AudioQuality);
}
