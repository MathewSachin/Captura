using System.Drawing;

namespace Captura
{
    public class StepsSettings : PropertyStore
    {
        public bool Enabled
        {
            get => Get(false);
            set => Set(value);
        }

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