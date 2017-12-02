using System;
using Captura.Webcam;

namespace Captura.Models
{
    public class WebcamItem : NotifyPropertyChanged, IWebcamItem
    {
        WebcamItem()
        {
            Name = LanguageManager.NoWebcam;

            TranslationSource.Instance.PropertyChanged += (s, e) =>
            {
                Name = LanguageManager.NoWebcam;

                RaisePropertyChanged(nameof(Name));
            };
        }

        public WebcamItem(Filter Cam)
        {
            this.Cam = Cam ?? throw new ArgumentNullException(nameof(Cam));
            Name = Cam.Name;
        }

        public static WebcamItem NoWebcam { get; } = new WebcamItem();

        public Filter Cam { get; }

        public string Name { get; private set; }

        public override string ToString() => Name;
    }
}