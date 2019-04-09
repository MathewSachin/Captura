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

        const int Offset = 20;

        public AroundMouseImageProvider(int Width,
            int Height,
            IPlatformServices PlatformServices,
            bool IncludeCursor)
        {
            _platformServices = PlatformServices;

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

        public IEditableFrame Capture()
        {
            var screenBounds = _platformServices.DesktopRectangle;
            var cursorPos = _platformServices.CursorPosition;

            var region = new Rectangle(_regionAroundMouse.Location, new Size(Width, Height));
            region.Inflate(-Offset, -Offset);

            if (!region.Contains(cursorPos))
            {
                // Shift Region
                _regionAroundMouse = new Rectangle(cursorPos.X - Width / 2, cursorPos.Y - Height / 2, Width, Height);

                if (_regionAroundMouse.Right > screenBounds.Right)
                {
                    _regionAroundMouse.X = screenBounds.Right - Width;
                }

                if (_regionAroundMouse.Bottom > screenBounds.Bottom)
                {
                    _regionAroundMouse.Y = screenBounds.Bottom - Height;
                }

                if (_regionAroundMouse.X < screenBounds.X)
                {
                    _regionAroundMouse.X = screenBounds.X;
                }

                if (_regionAroundMouse.Y < screenBounds.Y)
                {
                    _regionAroundMouse.Y = screenBounds.Y;
                }
            }

            return _regionProvider.Capture();
        }

        public void Dispose()
        {
            _regionProvider.Dispose();
        }
    }
}
