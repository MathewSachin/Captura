using System.Collections.Generic;

namespace Captura.FFmpeg
{
    public class FFmpegArgsBuilder
    {
        readonly List<FFmpegInputArgs> _inputs = new List<FFmpegInputArgs>();
        readonly List<FFmpegOutputArgs> _outputs = new List<FFmpegOutputArgs>();

        const string PipePrefix = @"\\.\pipe\";

        public FFmpegInputArgs AddInputFile(string FileName)
        {
            var input = new FFmpegInputArgs($"\"{FileName}\"");

            _inputs.Add(input);

            return input;
        }

        public FFmpegInputArgs AddStdIn()
        {
            var input = new FFmpegInputArgs("-");

            _inputs.Add(input);

            return input;
        }

        public FFmpegInputArgs AddInputPipe(string NamedPipe)
        {
            var input = new FFmpegInputArgs($"{PipePrefix}{NamedPipe}");

            _inputs.Add(input);

            return input;
        }

        public FFmpegOutputArgs AddOutputFile(string FileName)
        {
            var output = new FFmpegOutputArgs($"\"{FileName}\"");

            _outputs.Add(output);

            return output;
        }

        public FFmpegOutputArgs AddStdOut()
        {
            var output = new FFmpegOutputArgs("-");

            _outputs.Add(output);

            return output;
        }

        public FFmpegOutputArgs AddOutputPipe(string NamedPipe)
        {
            var output = new FFmpegOutputArgs($"{PipePrefix}{NamedPipe}");

            _outputs.Add(output);

            return output;
        }

        public string GetArgs()
        {
            var args = new List<string>();

            foreach (var input in _inputs)
            {
                args.Add(input.GetArgs());
            }

            foreach (var output in _outputs)
            {
                args.Add(output.GetArgs());
            }

            return string.Join(" ", args);
        }
    }
}