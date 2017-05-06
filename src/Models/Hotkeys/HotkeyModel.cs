using System.Windows.Forms;

namespace Captura
{
    public class HotkeyModel
    {
        public HotkeyModel(ServiceName ServiceName, Keys Key, Modifiers Modifiers)
        {
            this.ServiceName = ServiceName;
            this.Key = Key;
            this.Modifiers = Modifiers;
        }

        public ServiceName ServiceName { get; set; }

        public Keys Key { get; set; }

        public Modifiers Modifiers { get; set; }
    }
}
