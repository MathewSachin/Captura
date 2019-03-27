using System;

namespace Captura.Models
{
    public interface IPreviewWindow : IDisposable
    {
        void Display(IBitmapFrame Frame);

        void Show();

        bool IsVisible { get; }
    }
}