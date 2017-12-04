namespace Captura.Models
{
    public class NoSoundItem : NotifyPropertyChanged, IAudioItem
    {
        public static NoSoundItem Instance { get; } = new NoSoundItem();

        NoSoundItem()
        {
            TranslationSource.Instance.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(Name));
        }

        public string Name => LanguageManager.NoAudio;

        public override string ToString() => Name;
    }
}