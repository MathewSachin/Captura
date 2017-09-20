using System;
using Captura.Properties;
using WebEye.Controls.Wpf;

namespace Captura.Models
{
    public class WebcamItem : NotifyPropertyChanged, IWebcamItem
    {
        WebcamItem()
        {
            Name = Resources.NoWebcam;

            TranslationSource.Instance.PropertyChanged += (s, e) =>
            {
                Name = Resources.NoWebcam;

                RaisePropertyChanged(nameof(Name));
            };
        }

        public WebcamItem(WebCameraId Cam)
        {
            this.Cam = Cam ?? throw new ArgumentNullException(nameof(Cam));
            Name = Cam.Name;
        }

        public static WebcamItem NoWebcam { get; } = new WebcamItem();

        public WebCameraId Cam { get; }

        public string Name { get; private set; }

        public override string ToString() => Name;
    }
}