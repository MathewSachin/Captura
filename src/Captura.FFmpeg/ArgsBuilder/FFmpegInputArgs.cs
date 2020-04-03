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

        public FFmpegInputArgs AddArg(string Arg)
        {
            Args.Add(Arg);

            return this;
        }

        public FFmpegInputArgs AddArg<T>(string Key, T Value)
        {
            return AddArg($"-{Key} {Value}");
        }

        public FFmpegInputArgs SetVideoSize(int Width, int Height)
        {
            return AddArg("video_size", $"{Width}x{Height}");
        }

        public FFmpegInputArgs SetFrameRate(int FrameRate)
        {
            return AddArg("r", FrameRate);
        }

        public FFmpegInputArgs SetFormat(string Format)
        {
            return AddArg("f", Format);
        }

        public FFmpegInputArgs SetAudioCodec(string Codec)
        {
            return AddArg("acodec", Codec);
        }

        public FFmpegInputArgs SetAudioFrequency(int Frequency)
        {
            return AddArg("ar", Frequency);
        }

        public FFmpegInputArgs SetAudioChannels(int Channels)
        {
            return AddArg("ac", Channels);
        }

        public FFmpegInputArgs DisableVideo()
        {
            return AddArg("-vn");
        }
    }
}