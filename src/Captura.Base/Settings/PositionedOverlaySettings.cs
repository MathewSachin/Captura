// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Captura.Video
{
    public class PositionedOverlaySettings : PropertyStore
    {
        public Alignment HorizontalAlignment
        {
            get => Get(Alignment.Start);
            set => Set(value);
        }

        public Alignment VerticalAlignment
        {
            get => Get(Alignment.End);
            set => Set(value);
        }

        public int X
        {
            get => Get(80);
            set => Set(value);
        }

        public int Y
        {
            get => Get(100);
            set => Set(value);
        }
    }
}