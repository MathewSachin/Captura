using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Captura.ViewModels
{
    public class CustomOverlaysViewModel
    {
        readonly string _filePath;

        public CustomOverlaysViewModel()
        {
            Collection = new ReadOnlyObservableCollection<CustomOverlaySettings>(_collection);

            _filePath = Path.Combine(ServiceProvider.SettingsDir, "CustomOverlays.json");

            try
            {
                var json = File.ReadAllText(_filePath);

                var overlays = JsonConvert.DeserializeObject<CustomOverlaySettings[]>(json);

                foreach (var overlay in overlays)
                {
                    _collection.Add(overlay);
                }
            }
            catch
            {
                // Ignore Errors
            }

            AddCommand = new DelegateCommand(() => _collection.Add(new CustomOverlaySettings()));

            RemoveCommand = new DelegateCommand(OnRemoveExecute);
        }

        void OnRemoveExecute(object O)
        {
            if (O is CustomOverlaySettings textOverlaySettings)
            {
                _collection.Remove(textOverlaySettings);
            }
        }
        
        readonly ObservableCollection<CustomOverlaySettings> _collection = new ObservableCollection<CustomOverlaySettings>();

        public ReadOnlyObservableCollection<CustomOverlaySettings> Collection { get; }

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