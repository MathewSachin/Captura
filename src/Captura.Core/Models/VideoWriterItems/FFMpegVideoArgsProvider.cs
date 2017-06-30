namespace Captura.Models
{
    public class FFMpegVideoArgs
    {
        public FFMpegVideoArgs(string InArgs, string OutArgs)
        {
            InputArgs = InArgs;
            OutputArgs = OutArgs;
        }

        public string InputArgs { get; }
        
        public string OutputArgs { get; }
    }

    /// <summary>
    /// Provides Command-line args for FFMpeg Video encoding.
    /// </summary>
    /// <param name="VideoQuality">Video Quality... 1 to 100.</param>
    public delegate FFMpegVideoArgs FFMpegVideoArgsProvider(int VideoQuality);
}