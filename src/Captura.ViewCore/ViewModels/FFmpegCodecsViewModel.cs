using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegCodecsViewModel : NotifyPropertyChanged
    {
        public FFmpegSettings Settings { get; }

        public FFmpegCodecsViewModel(FFmpegSettings Settings)
        {
            this.Settings = Settings;

            AddCustomCodecCommand = new ReactiveCommand()
                .WithSubscribe(() => Settings.CustomCodecs.Add(new FFmpegCodecSettings()));

            RemoveCustomCodecCommand = new ReactiveCommand<FFmpegCodecSettings>()
                .WithSubscribe(M => Settings.CustomCodecs.Remove(M));
        }

        public ICommand AddCustomCodecCommand { get; }

        public ICommand RemoveCustomCodecCommand { get; }

        public IEnumerable<string> AudioCodecNames => FFmpegAudioItem.Items.Select(M => M.Name);
    }
}