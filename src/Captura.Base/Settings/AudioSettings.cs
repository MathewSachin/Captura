namespace Captura.Audio
{
    public class AudioSettings : PropertyStore
    {
        public bool SeparateFilePerSource
        {
            get => Get(false);
            set => Set(value);
        }

        public bool RecordMicrophone
        {
            get => Get(false);
            set => Set(value);
        }

        public string Microphone
        {
            get => Get("");
            set => Set(value);
        }

        public bool RecordSpeaker
        {
            get => Get(false);
            set => Set(value);
        }

        public string Speaker
        {
            get => Get("");
            set => Set(value);
        }

        public int Quality
        {
            get => Get(80);
            set => Set(value);
        }
    }
}