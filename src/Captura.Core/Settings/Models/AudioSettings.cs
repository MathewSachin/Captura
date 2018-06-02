namespace Captura
{
    public class AudioSettings : PropertyStore
    {
        public string[] Microphones
        {
            get => Get(new string[0]);
            set => Set(value);
        }

        public string[] Speakers
        {
            get => Get(new string[0]);
            set => Set(value);
        }

        public int Quality
        {
            get => Get(50);
            set => Set(value);
        }
    }
}