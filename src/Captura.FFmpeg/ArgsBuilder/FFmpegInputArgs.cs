namespace Captura.FFmpeg
{
    public class FFmpegInputArgs : FFmpegArgs
    {
        readonly string _input;

        public FFmpegInputArgs(string Input)
        {
            _input = Input;
        }

        public override string GetArgs()
        {
            return base.GetArgs() + $" -i {_input}";
        }

        public FFmpegInputArgs SetVideoSize(int Width, int Height)
        {
            Args.Add($"-s {Width}x{Height}");

            return this;
        }
    }
}