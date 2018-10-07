using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SoundsViewModel : NotifyPropertyChanged
    {
        readonly IDialogService _dialogService;

        public SoundSettings Settings { get; }

        public SoundsViewModel(IDialogService DialogService, SoundSettings Settings)
        {
            _dialogService = DialogService;
            this.Settings = Settings;

            ResetNormalCommand = new DelegateCommand(() => Settings.Normal = null);
            ResetShotCommand = new DelegateCommand(() => Settings.Shot = null);
            ResetErrorCommand = new DelegateCommand(() => Settings.Error = null);
            ResetNotificationCommand = new DelegateCommand(() => Settings.Notification = null);
        }

        public ICommand ResetNormalCommand { get; }
        public ICommand ResetShotCommand { get; }
        public ICommand ResetErrorCommand { get; }
        public ICommand ResetNotificationCommand { get; }
    }
}