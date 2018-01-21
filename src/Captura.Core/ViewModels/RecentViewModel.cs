using Captura.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Captura.ViewModels
{
    public class RecentViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<RecentItemViewModel> RecentList { get; } = new ObservableCollection<RecentItemViewModel>();
        
        public ICommand ClearCommand { get; }

        readonly string _filePath;
        readonly Settings _settings;

        public RecentViewModel(Settings Settings) : base(Settings)
        {
            _settings = Settings;
            _filePath = Path.Combine(ServiceProvider.SettingsDir, "RecentItems.json");

            try
            {
                var json = File.ReadAllText(_filePath);

                var list = JsonConvert.DeserializeObject<RecentItemModel[]>(json)
                    .Reverse() // Reversion required to maintain order
                    .Where(M => M.ItemType == RecentItemType.Link ||
                                File.Exists(M.FilePath)); // Restore only if file exists

                foreach (var model in list)
                {
                    Add(model.FilePath, model.ItemType, false);
                }
            }
            catch
            {
                // Ignore Errors
            }

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
            // Persist only if File exists or is a link.
            var items = RecentList.Where(M => M.ItemType == RecentItemType.Link && !M.IsSaving || File.Exists(M.FilePath))
                .Select(M => new RecentItemModel(M.FilePath, M.ItemType))
                .Take(_settings.RecentMax);

            try
            {
                var json = JsonConvert.SerializeObject(items);

                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Ignore Errors
            }
        }
    }
}