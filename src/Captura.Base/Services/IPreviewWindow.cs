using System;

namespace Captura.Video
{
    public interface IPreviewWindow : IDisposable
    {
        void Display(IBitmapFrame Frame);

        void Show();

        bool IsVisible { get; }
    }
}