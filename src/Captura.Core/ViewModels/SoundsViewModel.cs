using System.IO;
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

            SetNormalCommand = new DelegateCommand(OnSetNormalExecute);
            SetShotCommand = new DelegateCommand(OnSetShotExecute);
            SetErrorCommand = new DelegateCommand(OnSetErrorExecute);
            SetNotificationCommand = new DelegateCommand(OnSetNotificationExecute);
        }

        public ICommand ResetNormalCommand { get; }
        public ICommand ResetShotCommand { get; }
        public ICommand ResetErrorCommand { get; }
        public ICommand ResetNotificationCommand { get; }

        public ICommand SetNormalCommand { get; }
        public ICommand SetShotCommand { get; }
        public ICommand SetErrorCommand { get; }
        public ICommand SetNotificationCommand { get; }

        void OnSetNormalExecute()
        {
            var folder = _dialogService.PickFile(Path.GetDirectoryName(Settings.Normal), "");

            if (folder != null)
                Settings.Normal = folder;
        }

        void OnSetShotExecute()
        {
            var folder = _dialogService.PickFile(Path.GetDirectoryName(Settings.Shot), "");

            if (folder != null)
                Settings.Shot = folder;
        }

        void OnSetErrorExecute()
        {
            var folder = _dialogService.PickFile(Path.GetDirectoryName(Settings.Error), "");

            if (folder != null)
                Settings.Error = folder;
        }

        void OnSetNotificationExecute()
        {
            var folder = _dialogService.PickFile(Path.GetDirectoryName(Settings.Notification), "");

            if (folder != null)
                Settings.Notification = folder;
        }
    }
}