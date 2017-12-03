namespace Captura
{
    public class WebcamOverlaySettings : PositionedOverlaySettings
    {
        public int Opacity { get; set; } = 100;

        bool _resize;

        public bool Resize
        {
            get => _resize;
            set
            {
                _resize = value;
                
                OnPropertyChanged();
            }
        }

        public int ResizeWidth { get; set; } = 320;

        public int ResizeHeight { get; set; } = 240;
    }
}