namespace Captura
{
    public class WebcamOverlaySettings : PositionedOverlaySettings
    {
        public int Opacity { get; set; } = 100;

        public bool Resize { get; set; }

        public int ResizeWidth { get; set; } = 320;

        public int ResizeHeight { get; set; } = 240;
    }
}