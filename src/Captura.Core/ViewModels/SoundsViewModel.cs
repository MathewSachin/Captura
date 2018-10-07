using Captura.Models;

namespace Captura.ViewModels
{
    public class SoundsViewModel : NotifyPropertyChanged
    {
        readonly IDialogService _dialogService;

        public SoundSettings Settings { get; }

        public SoundsViewModel(IDialogService DialogService, SoundSettings Settings)
        {
            _dialogService = DialogService;
            this.Settings = Settings;
        }
    }
}