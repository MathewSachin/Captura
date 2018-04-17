using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class CustomImageOverlaysViewModel : ArraySettingsViewModel<CustomImageOverlaySettings>
    {
        public CustomImageOverlaysViewModel() : base("CustomImageOverlays.json")
        {
            ChangeCommand = new DelegateCommand(Change);
        }

        public ICommand ChangeCommand { get; }

        void Change() { }
    }
}