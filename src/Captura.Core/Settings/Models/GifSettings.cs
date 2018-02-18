namespace Captura
{
    public class GifSettings : PropertyStore
    {
        public bool Repeat
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int RepeatCount
        {
            get => Get<int>();
            set => Set(value);
        }

        public bool VariableFrameRate
        {
            get => Get(true);
            set => Set(value);
        }
    }
}