using System.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoVideoSourceProvider : VideoSourceProviderBase
    {
        public NoVideoSourceProvider(LanguageManager Loc) : base(Loc)
        {
            Sources = new IVideoItem[] {WaveItem.Instance}
                .Concat(FFmpegAudioItem.Items)
                .ToArray();
        }

        public IVideoItem[] Sources { get; }

        IVideoItem _selectedSource = WaveItem.Instance;

        public IVideoItem SelectedSource
        {
            get => _selectedSource;
            set
            {
                _selectedSource = value;
                
                OnPropertyChanged();

                RaisePropertyChanged(nameof(Source));
            }
        }

        public override IVideoItem Source => _selectedSource;

        public override string Name => Loc.OnlyAudio;
    }
}