using System.Windows.Forms;

namespace Captura.Models
{
    public class HotkeyModel
    {
        public HotkeyModel(ServiceName ServiceName, Keys Key, Modifiers Modifiers, bool IsActive)
        {
            this.ServiceName = ServiceName;
            this.Key = Key;
            this.Modifiers = Modifiers;
            this.IsActive = IsActive;
        }

        // Default constructor required by Settings
        public HotkeyModel() { }

        public bool IsActive { get; set; }

        public ServiceName ServiceName { get; set; }

        public Keys Key { get; set; }

        public Modifiers Modifiers { get; set; }
    }
}
