namespace Captura
{
    public class AroundMouseSettings : PropertyStore
    {
        public int Width
        {
            get => Get(600);
            set => Set(value);
        }

        public int Height
        {
            get => Get(400);
            set => Set(value);
        }

        public int Margin
        {
            get => Get(30);
            set => Set(value);
        }
    }
}