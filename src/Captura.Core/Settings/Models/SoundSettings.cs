namespace Captura.Models
{
    public class SoundSettings : PropertyStore
    {
        public string Normal
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Shot
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Error
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}