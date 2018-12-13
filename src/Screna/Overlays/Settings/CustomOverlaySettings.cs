namespace Captura
{
    public class CustomOverlaySettings : TextOverlaySettings
    {
        public string Text
        {
            get => Get("");
            set => Set(value);
        }
    }
}