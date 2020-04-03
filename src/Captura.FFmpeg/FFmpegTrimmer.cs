using System;
using System.Threading.Tasks;

namespace Captura.FFmpeg
{
    public class FFmpegTrimmer
    {
        public async Task Run(string SourceFile,
            TimeSpan From,
            TimeSpan To,
            string DestFile,
            bool HasAudio)
        {
            var argsBuilder = new FFmpegArgsBuilder();

            var inputArgs = argsBuilder.AddInputFile(SourceFile)
                .AddArg("ss", From)
                .AddArg("to", To);

            if (HasAudio)
                inputArgs.SetAudioCodec("copy");

            argsBuilder.AddOutputFile(DestFile);

            var args = argsBuilder.GetArgs();

            var process = FFmpegService.StartFFmpeg(args, DestFile, out _);

            await Task.Factory.StartNew(process.WaitForExit);

            if (process.ExitCode != 0)
            {
                throw new FFmpegException(process.ExitCode);
            }
        }
    }
}