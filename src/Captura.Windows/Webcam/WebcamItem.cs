using System;

namespace Captura.Webcam
{
    class WebcamItem : IWebcamItem
    {
        public WebcamItem(Filter Cam)
        {
            this.Cam = Cam ?? throw new ArgumentNullException(nameof(Cam));
            Name = Cam.Name;
        }

        public Filter Cam { get; }

        public string Name { get; }

        public IWebcamCapture BeginCapture(Action OnClick)
        {
            return new WebcamCapture(Cam, OnClick);
        }

        public override string ToString() => Name;
    }
}