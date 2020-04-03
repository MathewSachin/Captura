using SharpDX.DirectWrite;

namespace Captura.Windows.DirectX
{
    public class Direct2DFont : IFont
    {
        public Direct2DFont(string FontFamily, int Size, Factory DirectWriteFactory)
        {
            this.FontFamily = FontFamily;
            this.Size = Size;

            try
            {
                TextFormat = new TextFormat(DirectWriteFactory, FontFamily, Size);
            }
            catch
            {
                TextFormat = new TextFormat(DirectWriteFactory, "Arial", Size);
            }
        }

        public TextFormat TextFormat { get; }

        public void Dispose()
        {
            TextFormat.Dispose();
        }

        public int Size { get; }
        public string FontFamily { get; }
    }
}