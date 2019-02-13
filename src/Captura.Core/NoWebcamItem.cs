namespace Captura.Models
{
    public class NoWebcamItem : NotifyPropertyChanged, IWebcamItem
    {
        NoWebcamItem()
        {
            Name = LanguageManager.Instance.NoWebcam;

            LanguageManager.Instance.LanguageChanged += L =>
            {
                Name = LanguageManager.Instance.NoWebcam;

                RaisePropertyChanged(nameof(Name));
            };
        }

        public string Name { get; private set; }

        public IWebcamCapture BeginCapture() => null;

        public static IWebcamItem Instance { get; } = new NoWebcamItem();
    }
}