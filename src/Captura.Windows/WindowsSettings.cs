namespace Captura.Models
{
    public class WindowsSettings : PropertyStore
    {
        public bool UseGdi
        {
            get => Get(false);
            set => Set(value);
        }
    }
}