namespace Captura.Models
{
    public class NoSoundItem : NotifyPropertyChanged, IAudioItem
    {
        public static NoSoundItem Instance { get; } = new NoSoundItem();

        NoSoundItem()
        {
            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public string Name => LanguageManager.Instance.NoAudio;

        public override string ToString() => Name;
    }
}