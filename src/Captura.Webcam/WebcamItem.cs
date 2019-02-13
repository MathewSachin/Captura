using System;
using Captura.Webcam;

namespace Captura.Models
{
    public class WebcamItem : IWebcamItem
    {
        public WebcamItem(Filter Cam)
        {
            this.Cam = Cam ?? throw new ArgumentNullException(nameof(Cam));
            Name = Cam.Name;
        }

        public Filter Cam { get; }

        public string Name { get; }

        public IWebcamCapture BeginCapture()
        {
            return new WebcamCapture(Cam);
        }

        public override string ToString() => Name;
    }
}