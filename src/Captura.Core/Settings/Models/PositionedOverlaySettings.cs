// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Captura
{
    public abstract class PositionedOverlaySettings : NotifyPropertyChanged
    {
        public Alignment HorizontalAlignment { get; set; } = Alignment.Start;

        public Alignment VerticalAlignment { get; set; } = Alignment.End;

        public int X { get; set; } = 80;

        public int Y { get; set; } = 100;
    }
}