using System;
using System.Drawing;
using SharpDX.DXGI;

namespace Captura.Models
{
    public class DeskDuplItem : NotifyPropertyChanged, IVideoItem
    {
        readonly Adapter1 _adapter;
        readonly Output1 _output;

        public DeskDuplItem(Adapter1 Adapter, Output1 Output)
        {
            _adapter = Adapter;
            _output = Output;
        }

        public string Name => $"{_output.Description.DeviceName} ({_adapter.Description1.Description})";

        public override string ToString() => Name;

        public Rectangle Rectangle
        {
            get
            {
                var rawRect = _output.Description.DesktopBounds;

                return new Rectangle(rawRect.Left, rawRect.Top, rawRect.Right - rawRect.Left, rawRect.Bottom - rawRect.Top);
            }
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            var rect = Rectangle;
            rect.Location = Point.Empty;

            Transform = P => new Point(P.X - rect.Left, P.Y - rect.Top);

            return new DeskDuplImageProvider(_adapter, _output, rect, IncludeCursor);
        }
    }
}