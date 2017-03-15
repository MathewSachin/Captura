namespace Captura
{
    /// <summary>
    /// Provides Command-line args for FFMpeg
    /// </summary>
    /// <param name="VideoQuality">Video Quality... 1 to 100.</param>
    /// <param name="AudioCofig">Outputs Audio Config</param>
    /// <param name="VideoConfig">Outputs Video Config</param>
    public delegate void FFMpegArgsProvider(int VideoQuality, out string AudioCofig, out string VideoConfig);
}