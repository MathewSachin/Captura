using Screna.Audio;
using System;
using System.Diagnostics;
using System.IO;

namespace Captura.Models
{
    class FFMpegAudioWriter : IAudioFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;
        bool _error;

        public FFMpegAudioWriter(string FileName, int AudioQuality, FFMpegAudioArgsProvider AudioArgsProvider, int Frequency = 44100, int Channels = 2)
        {
            _ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = FFMpegService.FFMpegExePath,
                    Arguments = $"-hide_banner -f s16le -acodec pcm_s16le -ar {Frequency} -ac {Channels} -i - -vn {AudioArgsProvider(AudioQuality)} \"{FileName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };

            _ffmpegProcess.Exited += (s, e) => _error = _ffmpegProcess.ExitCode != 0;

            _ffmpegProcess.Start();
            
            _ffmpegIn = _ffmpegProcess.StandardInput.BaseStream;

            FFMpegLog.Instance.ReadLog(_ffmpegProcess.StandardError, "size=");
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
            if (_error)
            {
                throw new Exception("An Error Occured with FFMpeg");
            }

            _ffmpegIn.Write(Data, Offset, Count);
        }
    }
}
