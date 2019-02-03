namespace Captura
{
    public interface IClipboardService
    {
        void SetText(string Text);

        string GetText();

        bool HasText { get; }

        void SetImage(IBitmapImage Image);

        IBitmapImage GetImage();

        bool HasImage { get; }
    }
}