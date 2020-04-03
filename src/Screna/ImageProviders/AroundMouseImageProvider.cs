using System;
using System.Drawing;

namespace Captura.Video
{
    public class AroundMouseImageProvider : IImageProvider
    {
        readonly IPlatformServices _platformServices;
        readonly IImageProvider _regionProvider;
        Rectangle _regionAroundMouse;
        readonly int _margin;

        public AroundMouseImageProvider(int Width,
            int Height,
            int Margin,
            IPlatformServices PlatformServices,
            bool IncludeCursor)
        {
            _platformServices = PlatformServices;
            _margin = Margin;

            this.Width = Width;
            this.Height = Height;

            _regionAroundMouse = new Rectangle(0, 0, Width, Height);

            PointTransform = P => new Point(P.X - _regionAroundMouse.X, P.Y - _regionAroundMouse.Y);            

            _regionProvider = PlatformServices.GetRegionProvider(_regionAroundMouse,
                IncludeCursor,
                () => _regionAroundMouse.Location);
        }

        public int Height { get; }

        public int Width { get; }

        public Func<Point, Point> PointTransform { get; }

        void ShiftRegion(Point CursorPos, Rectangle OffsetRegion)
        {
            if (CursorPos.X < OffsetRegion.X)
            {
                _regionAroundMouse.X = CursorPos.X - _margin;
            }

            if (CursorPos.Y < OffsetRegion.Y)
            {
                _regionAroundMouse.Y = CursorPos.Y - _margin;
            }

            if (CursorPos.X > OffsetRegion.Right)
            {
                _regionAroundMouse.X = CursorPos.X - OffsetRegion.Width - _margin;
            }

            if (CursorPos.Y > OffsetRegion.Bottom)
            {
                _regionAroundMouse.Y = CursorPos.Y - OffsetRegion.Height - _margin;
            }
        }

        void CheckBounds(Rectangle ScreenBounds)
        {
            if (_regionAroundMouse.Right > ScreenBounds.Right)
            {
                _regionAroundMouse.X = ScreenBounds.Right - Width;
            }

            if (_regionAroundMouse.Bottom > ScreenBounds.Bottom)
            {
                _regionAroundMouse.Y = ScreenBounds.Bottom - Height;
            }

            if (_regionAroundMouse.X < ScreenBounds.X)
            {
                _regionAroundMouse.X = ScreenBounds.X;
            }

            if (_regionAroundMouse.Y < ScreenBounds.Y)
            {
                _regionAroundMouse.Y = ScreenBounds.Y;
            }
        }

        public IEditableFrame Capture()
        {
            var cursorPos = _platformServices.CursorPosition;

            var offsetRegion = new Rectangle(_regionAroundMouse.Location, _regionAroundMouse.Size);
            offsetRegion.Inflate(-_margin, -_margin);

            if (!offsetRegion.Contains(cursorPos))
            {
                ShiftRegion(cursorPos, offsetRegion);

                var screenBounds = _platformServices.DesktopRectangle;
                CheckBounds(screenBounds);
            }

            return _regionProvider.Capture();
        }

        public void Dispose()
        {
            _regionProvider.Dispose();
        }

        public IBitmapFrame DummyFrame => _regionProvider.DummyFrame;
    }
}
