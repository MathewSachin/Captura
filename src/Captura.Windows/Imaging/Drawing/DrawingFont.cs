using System.Drawing;
using Captura;

namespace Screna
{
    public class DrawingFont : IFont
    {
        public DrawingFont(string FontFamily, int Size)
        {
            this.FontFamily = FontFamily;
            this.Size = Size;

            try
            {
                Font = new Font(new FontFamily(FontFamily), Size, GraphicsUnit.Pixel);
            }
            catch
            {
                Font = new Font(System.Drawing.FontFamily.GenericMonospace, Size, GraphicsUnit.Pixel);
            }
        }

        public Font Font { get; }

        public void Dispose()
        {
            Font.Dispose();
        }

        public int Size { get; }
        public string FontFamily { get; }
    }
}