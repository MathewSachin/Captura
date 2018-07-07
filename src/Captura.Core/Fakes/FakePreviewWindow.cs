namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
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