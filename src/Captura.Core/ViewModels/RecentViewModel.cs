using Captura.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace Captura.ViewModels
{
    public class RecentViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<RecentItemViewModel> RecentList { get; } = new ObservableCollection<RecentItemViewModel>();
        
        public ICommand ClearCommand { get; }

        public RecentViewModel()
        {
            if (Settings.Instance.RecentItems == null)
                Settings.Instance.RecentItems = new List<RecentItemModel>();

            // Reversion required to maintain order.
            Settings.Instance.RecentItems.Reverse();

            // Restore only if File exists.
            foreach (var recent in Settings.Instance.RecentItems)
                if (File.Exists(recent.FilePath))
                    Add(recent.FilePath, recent.ItemType, false);

            ClearCommand = new DelegateCommand(() => RecentList.Clear());
        }

        public RecentItemViewModel Add(string FilePath, RecentItemType ItemType, bool IsSaving)
        {
            var item = new RecentItemViewModel(FilePath, ItemType, IsSaving);

            // Insert on Top
            RecentList.Insert(0, item);

            item.OnRemove += () => RecentList.Remove(item);

            return item;
        }

        public void Dispose()
        {
            Settings.Instance.RecentItems.Clear();

            var max = Settings.Instance.RecentMax;

            // Persist only if File exists.
            for (int i = 0; i < RecentList.Count && i < max; ++i)
                if (File.Exists(RecentList[i].FilePath))
                    Settings.Instance.RecentItems.Add(new RecentItemModel(RecentList[i].FilePath, RecentList[i].ItemType));
        }
    }
}