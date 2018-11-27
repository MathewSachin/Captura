using System.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoVideoSourceProvider : VideoSourceProviderBase
    {
        public NoVideoSourceProvider(LanguageManager Loc,
            IIconSet Icons) : base(Loc)
        {
            Sources = new IVideoItem[] {WaveItem.Instance}
                .Concat(FFmpegAudioItem.Items)
                .ToArray();

            Icon = Icons.NoVideo;
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

        public override string Description { get; } = @"No Video recorded.
Can be used for audio-only recording.
Make sure Audio sources are enabled.";

        public override string Icon { get; }

        public override bool Deserialize(string Serialized)
        {
            var source = Sources.FirstOrDefault(M => M.Name == Serialized);

            if (source == null)
                return false;

            SelectedSource = source;

            return true;
        }

        public override bool ParseCli(string Arg)
        {
            return Arg == "none";
        }
    }
}