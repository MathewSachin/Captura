using Captura.Audio;
using System.Collections.Generic;
using System.Linq;
using Captura.Loc;

namespace Captura.Video
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoVideoSourceProvider : VideoSourceProviderBase
    {
        public NoVideoSourceProvider(ILocalizationProvider Loc,
            IIconSet Icons,
            IEnumerable<IAudioWriterItem> AudioWriterItems) : base(Loc)
        {
            Sources = AudioWriterItems
                .Select(M => new NoVideoItem(M))
                .ToArray<IVideoItem>();

            Icon = Icons.NoVideo;

            if (Sources.Length > 0)
                SelectedSource = Sources[0];
        }

        public override bool SupportsStepsMode => false;

        public IVideoItem[] Sources { get; }

        IVideoItem _selectedSource;

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

        public override IBitmapImage Capture(bool IncludeCursor) => null;
    }
}