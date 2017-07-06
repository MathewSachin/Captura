using Captura.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Captura.ViewModels
{
    public class RecentItemViewModel : ViewModelBase
    {
        public RecentItemViewModel(string FilePath, RecentItemType ItemType, bool IsSaving)
        {
            this.FilePath = FilePath;

            Display = ItemType == RecentItemType.Link ? FilePath : Path.GetFileName(FilePath);

            this.IsSaving = IsSaving;
            this.ItemType = ItemType;

            InitCommands();
        }

        void InitCommands()
        {
            RemoveCommand = new DelegateCommand(() => OnRemove?.Invoke(), !IsSaving);

            OpenCommand = new DelegateCommand(() =>
            {
                ServiceProvider.LaunchFile(new ProcessStartInfo(FilePath));
            }, !IsSaving);

            PrintCommand = new DelegateCommand(() =>
            {
                ServiceProvider.LaunchFile(new ProcessStartInfo(FilePath) { Verb = "Print" });
            }, CanPrint);

            DeleteCommand = new DelegateCommand(() =>
            {
                if (!ServiceProvider.Messenger.ShowYesNo($"Are you sure you want to Delete: {FilePath}?", "Confirm Deletion"))
                    return;

                try
                {
                    File.Delete(FilePath);

                    // Remove from List
                    OnRemove?.Invoke();
                }
                catch (Exception E)
                {
                    ServiceProvider.Messenger.ShowError($"Could not Delete file: {FilePath}\n\n\n{E}");
                }
            }, !IsSaving);
        }

        bool CanPrint => !IsSaving && ItemType == RecentItemType.Image;

        string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;

                OnPropertyChanged();
            }
        }

        string _display;

        public string Display
        {
            get => _display;
            set
            {
                _display = value;

                OnPropertyChanged();
            }
        }

        public RecentItemType ItemType { get; }

        bool _saving;

        public bool IsSaving
        {
            get { return _saving; }
            private set
            {
                _saving = value;

                OnPropertyChanged();
            }
        }

        public void Saved()
        {
            if (IsSaving == false)
                return;

            IsSaving = false;

            (RemoveCommand as DelegateCommand).RaiseCanExecuteChanged(true);
            (OpenCommand as DelegateCommand).RaiseCanExecuteChanged(true);
            (DeleteCommand as DelegateCommand).RaiseCanExecuteChanged(true);

            (PrintCommand as DelegateCommand).RaiseCanExecuteChanged(CanPrint);
        }

        public ICommand RemoveCommand { get; private set; }

        public ICommand OpenCommand { get; private set; }

        public ICommand PrintCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public event Action OnRemove;
    }
}