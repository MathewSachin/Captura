using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class FFMpegCodecsViewModel : ViewModelBase
    {
        public FFMpegCodecsViewModel(Settings Settings, LanguageManager LanguageManager)
            : base(Settings, LanguageManager)
        {
            AddCustomCodecCommand = new DelegateCommand(() => Settings.FFMpeg.CustomCodecs.Add(new CustomFFMpegCodec()));

            RemoveCustomCodecCommand = new DelegateCommand(M =>
            {
                if (M is CustomFFMpegCodec codec)
                {
                    Settings.FFMpeg.CustomCodecs.Remove(codec);
                }
            });
        }

        public ICommand AddCustomCodecCommand { get; }

        public ICommand RemoveCustomCodecCommand { get; }

        public IEnumerable<string> AudioCodecNames => FFMpegAudioItem.Items.Select(M => M.Name.Split(' ')[0]);
    }
}