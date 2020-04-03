using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Captura.Video
{
    public class ImageFolderWriter : IVideoFileWriter
    {
        readonly string _folderPath;
        int _index;

        public ImageFolderWriter(string OutputPath)
        {
            _folderPath = OutputPath;

            Directory.CreateDirectory(_folderPath);
        }

        public void Dispose() { }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
                return;

            using (Image)
            {
                // TODO: Make independent of System.Drawing
                using var bmp = new Bitmap(Image.Width, Image.Height);
                var data = bmp.LockBits(new Rectangle(Point.Empty, new Size(Image.Width, Image.Height)), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    Image.CopyTo(data.Scan0);
                }
                finally
                {
                    bmp.UnlockBits(data);
                }

                var filePath = Path.Combine(_folderPath, $"{_index:D3}.png");

                bmp.Save(filePath, ImageFormat.Png);
            }

            ++_index;
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Offset, int Length) { }
    }
}