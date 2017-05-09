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

            FileName = Path.GetFileName(FilePath);

            this.IsSaving = IsSaving;
            this.ItemType = ItemType;

            RemoveCommand = new DelegateCommand(() => OnRemove?.Invoke(), !IsSaving);

            OpenCommand = new DelegateCommand(() =>
            {
                try { Process.Start(FilePath); }
                catch
                {
                    // TODO: Show MessageBox Service - file not found
                }
            }, !IsSaving);

            PrintCommand = new DelegateCommand(() =>
            {
                try { Process.Start(new ProcessStartInfo(FilePath) { Verb = "Print" }); }
                catch
                {
                    // Suppress error
                }
            }, CanPrint);

            DeleteCommand = new DelegateCommand(() =>
            {
                try { File.Delete(FilePath); }
                catch
                {
                    //MessageBox.Show($"Can't Delete {FilePath}. It will still be removed from list.", "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                OnRemove?.Invoke();
            }, !IsSaving);
        }

        bool CanPrint => !IsSaving && ItemType == RecentItemType.Image;

        public string FilePath { get; }

        public string FileName { get; }

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

        public ICommand RemoveCommand { get; }

        public ICommand OpenCommand { get; }

        public ICommand PrintCommand { get; }

        public ICommand DeleteCommand { get; }

        public event Action OnRemove;
    }
}