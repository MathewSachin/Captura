using System;
using Screna;

namespace Captura.Models
{
    public interface IPreviewWindow : IDisposable
    {
        void Init(int Width, int Height);

        void Display(IBitmapFrame Frame);

        void Show();
    }
}