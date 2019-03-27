using System.IO;

namespace Captura
{
    public interface IImagingSystem
    {
        IBitmapImage CreateBitmap(int Width, int Height);

        IBitmapImage LoadBitmap(string FileName);

        IBitmapImage LoadBitmap(Stream Stream);
    }
}