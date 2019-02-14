using System;

namespace Captura.Models
{
    public interface IWebcamItem
    {
        string Name { get; }

        IWebcamCapture BeginCapture(Action OnClick);
    }
}