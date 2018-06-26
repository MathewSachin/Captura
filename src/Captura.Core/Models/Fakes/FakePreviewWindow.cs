using Screna;

namespace Captura.Models
{
    public class FakePreviewWindow : IPreviewWindow
    {
        public void Dispose() { }

        public void Init(int Width, int Height) { }

        public void Display(IBitmapFrame Frame)
        {
            Frame.Dispose();
        }

        public void Show() { }
    }
}