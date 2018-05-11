using System;
using Screna;

namespace Captura.Models
{
    public interface IPreviewWindow : IDisposable
    {
        void Display(IBitmapFrame Frame);

        void Show();
    }
}