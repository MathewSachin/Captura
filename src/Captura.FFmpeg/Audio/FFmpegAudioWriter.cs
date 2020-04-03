using Captura.Audio;
using System.Diagnostics;
using System.IO;

namespace Captura.FFmpeg
{
    class FFmpegAudioWriter : IAudioFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;
        
        public FFmpegAudioWriter(string FileName, int AudioQuality, FFmpegAudioArgsProvider AudioArgsProvider, int Frequency = 44100, int Channels = 2)
        {
            if (!FFmpegService.FFmpegExists)
            {
                throw new FFmpegNotFoundException();
            }

            var argsBuilder  = new FFmpegArgsBuilder();

            argsBuilder.AddStdIn()
                .SetFormat("s16le")
                .SetAudioCodec("pcm_s16le")
                .SetAudioFrequency(Frequency)
                .SetAudioChannels(Channels)
                .DisableVideo();

            var output = argsBuilder.AddOutputFile(FileName);

            AudioArgsProvider(AudioQuality, output);

            _ffmpegProcess = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), FileName, out _);
            
            _ffmpegIn = _ffmpegProcess.StandardInput.BaseStream;
        }

        public void Dispose()
        {
            Flush();

            _ffmpegIn.Close();
            _ffmpegProcess.WaitForExit();
        }

        public void Flush()
        {
            _ffmpegIn.Flush();
        }

        public void Write(byte[] Data, int Offset, int Count)
        {
            if (_ffmpegProcess.HasExited)
            {
                throw new FFmpegException(_ffmpegProcess.ExitCode);
            }

            _ffmpegIn.Write(Data, Offset, Count);
        }
    }
}
