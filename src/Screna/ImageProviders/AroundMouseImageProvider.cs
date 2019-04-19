using System;
using System.Drawing;
using Captura;

namespace Screna
{
    public class AroundMouseImageProvider : IImageProvider
    {
        readonly IPlatformServices _platformServices;
        readonly IImageProvider _regionProvider;
        Rectangle _regionAroundMouse;
        readonly int _offsetX, _offsetY;

        public AroundMouseImageProvider(int Width,
            int Height,
            IPlatformServices PlatformServices,
            bool IncludeCursor)
        {
            _platformServices = PlatformServices;

            this.Width = Width;
            this.Height = Height;

            _offsetX = Width / 4;
            _offsetY = Height / 4;

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
                _regionAroundMouse.X = CursorPos.X - _offsetX;
            }

            if (CursorPos.Y < OffsetRegion.Y)
            {
                _regionAroundMouse.Y = CursorPos.Y - _offsetY;
            }

            if (CursorPos.X > OffsetRegion.Right)
            {
                _regionAroundMouse.X = CursorPos.X - OffsetRegion.Width - _offsetX;
            }

            if (CursorPos.Y > OffsetRegion.Bottom)
            {
                _regionAroundMouse.Y = CursorPos.Y - OffsetRegion.Height - _offsetY;
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
            offsetRegion.Inflate(-_offsetX, -_offsetY);

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

        public Type EditorType => _regionProvider.EditorType;
    }
}
