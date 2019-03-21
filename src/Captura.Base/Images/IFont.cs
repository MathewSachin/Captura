using System;

namespace Captura
{
    public interface IFont : IDisposable
    {
        /// <summary>
        /// Font Size in Device Independent Pixels
        /// </summary>
        int Size { get; }

        string FontFamily { get; }
    }
}