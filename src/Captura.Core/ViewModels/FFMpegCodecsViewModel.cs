using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class FFmpegCodecsViewModel : ViewModelBase
    {
        public FFmpegCodecsViewModel(Settings Settings, LanguageManager LanguageManager)
            : base(Settings, LanguageManager)
        {
            AddCustomCodecCommand = new DelegateCommand(() => Settings.FFmpeg.CustomCodecs.Add(new CustomFFmpegCodec()));

            RemoveCustomCodecCommand = new DelegateCommand(M =>
            {
                if (M is CustomFFmpegCodec codec)
                {
                    Settings.FFmpeg.CustomCodecs.Remove(codec);
                }
            });
        }

        public ICommand AddCustomCodecCommand { get; }

        public ICommand RemoveCustomCodecCommand { get; }

        public IEnumerable<string> AudioCodecNames => FFmpegAudioItem.Items.Select(M => M.Name.Split(' ')[0]);
    }
}