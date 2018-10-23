using System.Drawing;
using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ClipboardService : IClipboardService
    {
        public void SetText(string Text) => Text.WriteToClipboard();

        public string GetText() => Clipboard.GetText();

        public bool HasText => Clipboard.ContainsText();

        public void SetImage(Image Image) => Image.WriteToClipboard();

        public Image GetImage() => Clipboard.GetImage();

        public bool HasImage => Clipboard.ContainsImage();
    }
}