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

            var emSize = Size * 72 / 96f;

            try
            {
                Font = new Font(new FontFamily(FontFamily), emSize);
            }
            catch
            {
                Font = new Font(System.Drawing.FontFamily.GenericMonospace, emSize);
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