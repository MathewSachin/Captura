using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Captura.ViewModels
{
    public class CustomOverlaysViewModel
    {
        CustomOverlaysViewModel()
        {
            if (Settings.Instance.CustomOverlays == null)
                Settings.Instance.CustomOverlays = new List<CustomOverlaySettings>();

            foreach (var overlay in Settings.Instance.CustomOverlays)
            {
                Collection.Add(overlay);
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
            Settings.Instance.CustomOverlays.Clear();

            foreach (var overlay in Collection)
            {
                // Save
                Settings.Instance.CustomOverlays.Add(overlay);
            }
        }
    }
}