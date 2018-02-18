namespace Captura
{
    public class AudioSettings : PropertyStore
    {
        public string Microphone
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Speaker
        {
            get => Get<string>();
            set => Set(value);
        }

        public int Quality
        {
            get => Get(50);
            set => Set(value);
        }
    }
}