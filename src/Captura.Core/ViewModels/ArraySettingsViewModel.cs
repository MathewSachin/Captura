using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Captura.ViewModels
{
    public class ArraySettingsViewModel<T> : IDisposable where T : new()
    {
        readonly string _filePath;

        public ArraySettingsViewModel(string FileName)
        {
            _filePath = Path.Combine(ServiceProvider.SettingsDir, FileName);

            Collection = new ReadOnlyObservableCollection<T>(_collection);

            Load();

            AddCommand = new DelegateCommand(() => _collection.Add(new T()));

            RemoveCommand = new DelegateCommand(OnRemoveExecute);
        }

        void Load()
        {
            try
            {
                var json = File.ReadAllText(_filePath);

                var overlays = JsonConvert.DeserializeObject<T[]>(json);

                foreach (var overlay in overlays)
                {
                    _collection.Add(overlay);
                }
            }
            catch
            {
                // Ignore Errors
            }
        }

        void OnRemoveExecute(object O)
        {
            if (O is T setting)
            {
                _collection.Remove(setting);
            }
        }

        readonly ObservableCollection<T> _collection = new ObservableCollection<T>();

        public ReadOnlyObservableCollection<T> Collection { get; }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

        public void Reset() => _collection.Clear();

        public void Dispose()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Collection);

                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Ignore Errors
            }
        }
    }
}