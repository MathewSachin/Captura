using System;

namespace Captura
{
    public interface IFont : IDisposable
    {
        int Size { get; }

        string FontFamily { get; }
    }
}