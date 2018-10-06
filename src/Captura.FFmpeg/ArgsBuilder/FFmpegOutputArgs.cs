namespace Captura.FFmpeg
{
    public class FFmpegOutputArgs : FFmpegArgs
    {
        readonly string _output;

        public FFmpegOutputArgs(string Output)
        {
            _output = Output;
        }

        public override string GetArgs()
        {
            return base.GetArgs() + $" {_output}";
        }

        public FFmpegOutputArgs SetVideoSize(int Width, int Height)
        {
            Args.Add($"-s {Width}x{Height}");

            return this;
        }
    }
}