using System;

namespace Captura.Webcam
{
    public interface IWebcamItem
    {
        string Name { get; }

        IWebcamCapture BeginCapture(Action OnClick);
    }
}