using Screna.Audio;
using System.Diagnostics;
using System.IO;

namespace Captura
{
    class FFMpegAudioWriter : IAudioFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;

        public FFMpegAudioWriter(string FileName, FFMpegAudioWriterItem FFMpegItem, int Frequency = 44100, int Channels = 2)
        {
            _ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-f s16le -acodec pcm_s16le -ar {Frequency} -ac {Channels} -i - -vn {FFMpegItem.AudioArgsProvider()} \"{FileName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true
                }
            };

            _ffmpegProcess.Start();

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
            _ffmpegIn.Write(Data, Offset, Count);
        }
    }
}
