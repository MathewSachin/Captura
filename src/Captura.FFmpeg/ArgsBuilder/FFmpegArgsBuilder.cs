using System.Collections.Generic;

namespace Captura.FFmpeg
{
    public class FFmpegArgsBuilder
    {
        readonly List<FFmpegInputArgs> _inputs = new List<FFmpegInputArgs>();
        readonly List<FFmpegOutputArgs> _outputs = new List<FFmpegOutputArgs>();

        public FFmpegInputArgs AddInput(string Input)
        {
            var input = new FFmpegInputArgs(Input);

            _inputs.Add(input);

            return input;
        }

        public FFmpegOutputArgs AddOutputArgs(string Output)
        {
            var output = new FFmpegOutputArgs(Output);

            _outputs.Add(output);

            return output;
        }

        public string GetArgs()
        {
            var args = "";

            foreach (var input in _inputs)
            {
                args += input.GetArgs() + " ";
            }

            foreach (var output in _outputs)
            {
                args += output.GetArgs() + " ";
            }

            return args;
        }
    }
}