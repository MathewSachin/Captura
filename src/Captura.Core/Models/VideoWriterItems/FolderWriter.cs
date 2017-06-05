using System.Drawing;
using Screna;
using Screna.Audio;
using System.IO;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Captura.Models
{
    class FolderWriter : IVideoFileWriter
    {
        public bool SupportsAudio { get; }

        readonly AudioFileWriter _audioWriter;

        string _folderPath;
        int fileIndex = 0;

        public FolderWriter(string FolderName, int FrameRate, IAudioProvider AudioProvider)
        {
            _folderPath = FolderName;
            SupportsAudio = AudioProvider != null;

            Directory.CreateDirectory(FolderName);

            File.WriteAllText(Path.Combine(FolderName, "FrameRate.txt"), FrameRate.ToString());

            if (SupportsAudio)
            {
                _audioWriter = new AudioFileWriter(Path.Combine(FolderName, "audio.wav"), AudioProvider.WaveFormat);
            }
        }

        public void Dispose()
        {
            _audioWriter?.Dispose();
        }

        public void WriteAudio(byte[] Buffer, int Length)
        {
            _audioWriter?.Write(Buffer, 0, Length);
        }

        public void WriteFrame(Bitmap Image)
        {
            var index = fileIndex++;

            Task.Factory.StartNew(() =>
            {
                Image.Save(Path.Combine(_folderPath, $"{index}.png"), ImageFormat.Png);
                Image.Dispose();
            });
        }
    }
}
