using Captura.Audio;
using System;
using System.Diagnostics;
using System.IO;

namespace Captura.Models
{
    class FFmpegAudioWriter : IAudioFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;
        
        public FFmpegAudioWriter(string FileName, int AudioQuality, FFmpegAudioArgsProvider AudioArgsProvider, int Frequency = 44100, int Channels = 2)
        {
            var argsBuilder  = new FFmpegArgsBuilder();

            argsBuilder.AddStdIn()
                .SetFormat("s16le")
                .SetAudioCodec("pcm_s16le")
                .SetAudioFrequency(Frequency)
                .SetAudioChannels(Channels)
                .DisableVideo();

            var output = argsBuilder.AddOutputFile(FileName);

            AudioArgsProvider(AudioQuality, output);

            _ffmpegProcess = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), FileName);
            
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
                throw new Exception("An Error Occurred with FFmpeg");
            }

            _ffmpegIn.Write(Data, Offset, Count);
        }
    }
}
