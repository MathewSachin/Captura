using System.Collections.Generic;
using System.Linq;
using Captura.Audio;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SoundsViewModel : NotifyPropertyChanged
    {
        public IReadOnlyCollection<SoundsViewModelItem> Items { get; }

        public SoundsViewModel(IDialogService DialogService, SoundSettings Settings)
        {
            Items = new[]
            {
                SoundKind.Start,
                SoundKind.Stop,
                SoundKind.Pause,
                SoundKind.Shot,
                SoundKind.Error,
                SoundKind.Notification
            }.Select(Kind => new SoundsViewModelItem(Kind, DialogService, Settings)).ToList();
        }
    }
}