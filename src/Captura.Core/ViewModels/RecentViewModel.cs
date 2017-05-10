using Captura.Models;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class RecentViewModel : ViewModelBase
    {
        public ObservableCollection<RecentItemViewModel> RecentList { get; } = new ObservableCollection<RecentItemViewModel>();

        public RecentItemViewModel Add(string FilePath, RecentItemType ItemType, bool IsSaving)
        {
            var item = new RecentItemViewModel(FilePath, ItemType, IsSaving);

            RecentList.Insert(0, item);

            item.OnRemove += () => RecentList.Remove(item);

            return item;
        }
    }
}