namespace Captura
{
    public class StepsSettings : PropertyStore
    {
        public string Writer
        {
            get => Get("");
            set => Set(value);
        }

        public bool IncludeScrolls
        {
            get => Get(true);
            set => Set(value);
        }
    }
}