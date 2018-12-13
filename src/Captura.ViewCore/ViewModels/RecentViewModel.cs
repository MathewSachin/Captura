﻿using Captura.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecentViewModel : ViewModelBase
    {
        public ReadOnlyObservableCollection<IRecentItem> Items { get; }

        public ICommand ClearCommand { get; }

        public RecentViewModel(Settings Settings,
            LanguageManager LanguageManager,
            IRecentList Recent)
            : base(Settings, LanguageManager)
        {
            Items = Recent.Items;

            ClearCommand = new DelegateCommand(Recent.Clear);
        }
    }
}