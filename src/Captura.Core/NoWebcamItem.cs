using System;

namespace Captura.Models
{
    public class NoWebcamItem : NotifyPropertyChanged, IWebcamItem
    {
        NoWebcamItem()
        {
            var loc = LanguageManager.Instance;

            Name = loc.NoWebcam;

            loc.LanguageChanged += L =>
            {
                Name = loc.NoWebcam;

                RaisePropertyChanged(nameof(Name));
            };
        }

        public string Name { get; private set; }

        public IWebcamCapture BeginCapture(Action OnClick) => null;

        public static IWebcamItem Instance { get; } = new NoWebcamItem();
    }
}