// Adapted from https://github.com/jasonpang/desktop-duplication-net

using DPoint = System.Drawing.Point;
using DRect = System.Drawing.Rectangle;
using SPoint = SharpDX.Point;
using SRect = SharpDX.Rectangle;

namespace DesktopDuplication
{
    public static class Extensions
    {
        public static DRect ToDRect(this SRect Rect)
        {
            return new DRect(Rect.X, Rect.Y, Rect.Width, Rect.Height);
        }

        public static DPoint ToDPoint(this SPoint P)
        {
            return new DPoint(P.X, P.Y);
        }
    }
}
