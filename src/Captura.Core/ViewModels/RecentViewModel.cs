using Captura.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecentViewModel : ViewModelBase, IRecentList
    {
        readonly ObservableCollection<IRecentItem> _recentList = new ObservableCollection<IRecentItem>();

        public ReadOnlyObservableCollection<IRecentItem> RecentList { get; }
        
        public ICommand ClearCommand { get; }

        readonly IEnumerable<IRecentItemSerializer> _recentItemSerializers;
        readonly Settings _settings;

        static string GetFilePath()
        {
            return Path.Combine(ServiceProvider.SettingsDir, "RecentItems.json");
        }

        public RecentViewModel(Settings Settings,
            LanguageManager LanguageManager,
            IEnumerable<IRecentItemSerializer> RecentItemSerializers)
            : base(Settings, LanguageManager)
        {
            RecentList = new ReadOnlyObservableCollection<IRecentItem>(_recentList);

            _settings = Settings;
            _recentItemSerializers = RecentItemSerializers;

            Load();

            ClearCommand = new DelegateCommand(Clear);
        }

        void Load()
        {
            try
            {
                var json = File.ReadAllText(GetFilePath());

                var jarray = JArray.Parse(json);

                var items = new List<IRecentItem>();

                foreach (var jItem in jarray)
                {
                    var jObj = (JObject) jItem;

                    var serializer = _recentItemSerializers.FirstOrDefault(M => M.CanDeserialize(jObj));

                    var item = serializer?.Deserialize(jObj);

                    if (item != null)
                    {
                        items.Add(item);
                    }
                }

                // Reversion required to maintain order
                items.Reverse();

                foreach (var model in items)
                {
                    Add(model);
                }
            }
            catch
            {
                // Ignore Errors
            }
        }

        public void Add(IRecentItem RecentItem)
        {
            // Insert on Top
            _recentList.Insert(0, RecentItem);

            RecentItem.RemoveRequested += () => _recentList.Remove(RecentItem);
        }

        IEnumerable<IRecentItem> IRecentList.Items => RecentList;

        public void Clear()
        {
            _recentList.Clear();
        }

        public void Dispose()
        {
            try
            {
                var items = new JArray();

                foreach (var item in RecentList)
                {
                    var serializer = _recentItemSerializers.FirstOrDefault(M => M.CanSerialize(item));

                    var jItem = serializer?.Serialize(item);

                    if (jItem != null)
                    {
                        items.Add(jItem);
                    }
                }

                var json = items.ToString();

                File.WriteAllText(GetFilePath(), json);
            }
            catch
            {
                // Ignore Errors
            }
        }
    }
}