using System.Collections.Generic;

namespace Captura.Models
{
    public class FFmpegArgsBuilder
    {
        readonly List<FFmpegInputArgs> _inputs = new List<FFmpegInputArgs>();
        readonly List<FFmpegOutputArgs> _outputs = new List<FFmpegOutputArgs>();

        public FFmpegInputArgs AddInputFile(string FileName)
        {
            var input = new FFmpegInputArgs($"\"{FileName}\"");

            _inputs.Add(input);

            return input;
        }

        public FFmpegInputArgs AddInputPipe(string PipeName = "-")
        {
            var input = new FFmpegInputArgs(PipeName);

            _inputs.Add(input);

            return input;
        }

        public FFmpegOutputArgs AddOutputFile(string FileName)
        {
            var output = new FFmpegOutputArgs($"\"{FileName}\"");

            _outputs.Add(output);

            return output;
        }

        public FFmpegOutputArgs AddOutputPipe(string PipeName = "-")
        {
            var output = new FFmpegOutputArgs(PipeName);

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