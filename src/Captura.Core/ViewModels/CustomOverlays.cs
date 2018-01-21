using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Captura.ViewModels
{
    public class CustomOverlaysViewModel
    {
        readonly string _filePath;

        CustomOverlaysViewModel()
        {
            _filePath = Path.Combine(ServiceProvider.SettingsDir, "CustomOverlays.json");

            try
            {
                var json = File.ReadAllText(_filePath);

                var overlays = JsonConvert.DeserializeObject<CustomOverlaySettings[]>(json);

                foreach (var overlay in overlays)
                {
                    Collection.Add(overlay);
                }
            }
            catch
            {
                // Ignore Errors
            }

            AddCommand = new DelegateCommand(() => Collection.Add(new CustomOverlaySettings()));

            RemoveCommand = new DelegateCommand(OnRemoveExecute);
        }

        void OnRemoveExecute(object O)
        {
            if (O is CustomOverlaySettings textOverlaySettings)
            {
                Collection.Remove(textOverlaySettings);
            }
        }

        public static CustomOverlaysViewModel Instance { get; } = new CustomOverlaysViewModel();

        public ObservableCollection<CustomOverlaySettings> Collection { get; } = new ObservableCollection<CustomOverlaySettings>();

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

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