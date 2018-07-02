using System.Drawing;

namespace Captura
{
    public interface IClipboardService
    {
        void SetText(string Text);

        string GetText();

        bool HasText { get; }

        void SetImage(Image Image);

        Image GetImage();

        bool HasImage { get; }
    }
}