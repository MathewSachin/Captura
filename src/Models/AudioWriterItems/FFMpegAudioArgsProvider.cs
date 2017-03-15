namespace Captura
{
    /// <summary>
    /// Provides FFMpeg Audio encoding Command-line args.
    /// </summary>
    /// <param name="AudioQuality">Audio Quality... 1 to 100.</param>
    /// <returns>FFMpeg Audio encoding Command-line args</returns>
    public delegate string FFMpegAudioArgsProvider(int AudioQuality);
}
