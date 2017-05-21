using Captura.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class RecentViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<RecentItemViewModel> RecentList { get; } = new ObservableCollection<RecentItemViewModel>();

        int maxItemsToPersist = 30;

        public RecentViewModel()
        {
            if (Settings.Instance.RecentItems == null)
                Settings.Instance.RecentItems = new List<RecentItemModel>();

            // Reversion required to maintain order.
            Settings.Instance.RecentItems.Reverse();

            foreach (var recent in Settings.Instance.RecentItems)
                Add(recent.FilePath, recent.ItemType, false);
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

            for (int i = 0; i < RecentList.Count && i < maxItemsToPersist; ++i)
                Settings.Instance.RecentItems.Add(new RecentItemModel(RecentList[i].FilePath, RecentList[i].ItemType));
        }
    }
}