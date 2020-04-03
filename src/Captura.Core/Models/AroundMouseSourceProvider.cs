using System.Drawing;
using System.Text.RegularExpressions;

namespace Captura.Video
{
    public class AroundMouseSourceProvider : IVideoSourceProvider
    {
        readonly Settings _settings;
        readonly IPlatformServices _platformServices;

        public AroundMouseSourceProvider(IIconSet Icons,
            AroundMouseItem AroundMouseItem,
            Settings Settings,
            IPlatformServices PlatformServices)
        {
            _settings = Settings;
            _platformServices = PlatformServices;

            Icon = Icons.Cursor;
            Source = AroundMouseItem;
        }

        public string Name => "Around Mouse";

        public string Description => "Capture region surrounding mouse";

        public string Icon { get; }

        public bool SupportsStepsMode => true;

        public IVideoItem Source { get; }

        public IBitmapImage Capture(bool IncludeCursor)
        {
            var cursorPos = _platformServices.CursorPosition;
            var screenBounds = _platformServices.DesktopRectangle;
            var w = _settings.AroundMouse.Width;
            var h = _settings.AroundMouse.Height;

            var region = new Rectangle(cursorPos.X - w / 2, cursorPos.Y - h / 2, w, h);

            if (region.Right > screenBounds.Right)
            {
                region.X = screenBounds.Right - w;
            }

            if (region.Bottom > screenBounds.Bottom)
            {
                region.Y = screenBounds.Bottom - h;
            }

            if (region.X < screenBounds.X)
            {
                region.X = screenBounds.X;
            }

            if (region.Y < screenBounds.Y)
            {
                region.Y = screenBounds.Y;
            }

            return ScreenShot.Capture(region, IncludeCursor);
        }

        const string Key = "aroundmouse";

        public bool Deserialize(string Serialized) => Serialized == Key;

        public bool OnSelect() => true;

        public void OnUnselect()
        {
        }

        public bool ParseCli(string Arg)
        {
            var regex = new Regex($@"^{Key}:(\d+),(\d+)$");
            var match = regex.Match(Arg);

            if (match.Success)
            {
                _settings.AroundMouse.Width = int.Parse(match.Groups[1].Value);
                _settings.AroundMouse.Height = int.Parse(match.Groups[2].Value);

                return true;
            }

            return false;
        }

        public string Serialize() => Key;
    }
}
