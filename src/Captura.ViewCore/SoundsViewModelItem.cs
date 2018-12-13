﻿using System.IO;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class SoundsViewModelItem : NotifyPropertyChanged
    {
        readonly SoundSettings _settings;

        public SoundsViewModelItem(SoundKind SoundKind, IDialogService DialogService, SoundSettings Settings)
        {
            this.SoundKind = SoundKind;
            _settings = Settings;

            ResetCommand = new DelegateCommand(() => FileName = null);

            SetCommand = new DelegateCommand(() =>
            {
                var folder = DialogService.PickFile(Path.GetDirectoryName(FileName), "");

                if (folder != null)
                    FileName = folder;
            });
        }

        public string FileName
        {
            get => _settings.Items.TryGetValue(SoundKind, out var value) ? value : null;
            set
            {
                if (_settings.Items.ContainsKey(SoundKind))
                {
                    _settings.Items[SoundKind] = value;
                }
                else _settings.Items.Add(SoundKind, value);

                OnPropertyChanged();
            }
        }

        public SoundKind SoundKind { get; }

        public ICommand ResetCommand { get; }

        public ICommand SetCommand { get; }
    }
}