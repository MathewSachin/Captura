namespace Captura.Models
{
    class FFmpegOutputArgs : FFmpegArgs
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

        public FFmpegOutputArgs AddArg(string Arg)
        {
            Args.Add(Arg);

            return this;
        }

        public FFmpegOutputArgs SetVideoSize(int Width, int Height)
        {
            return AddArg($"-video_size {Width}x{Height}");
        }

        public FFmpegOutputArgs SetFrameRate(int FrameRate)
        {
            return AddArg($"-r {FrameRate}");
        }
    }
}