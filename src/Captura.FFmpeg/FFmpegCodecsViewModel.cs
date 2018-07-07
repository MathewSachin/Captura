using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegCodecsViewModel : NotifyPropertyChanged
    {
        public FFmpegSettings Settings { get; }

        public FFmpegCodecsViewModel(FFmpegSettings Settings)
        {
            this.Settings = Settings;

            AddCustomCodecCommand = new DelegateCommand(() => Settings.CustomCodecs.Add(new CustomFFmpegCodec()));

            RemoveCustomCodecCommand = new DelegateCommand(M =>
            {
                if (M is CustomFFmpegCodec codec)
                {
                    Settings.CustomCodecs.Remove(codec);
                }
            });
        }

        public ICommand AddCustomCodecCommand { get; }

        public ICommand RemoveCustomCodecCommand { get; }

        public IEnumerable<string> AudioCodecNames => FFmpegAudioItem.Items.Select(M => M.Name.Split(' ')[0]);
    }
}