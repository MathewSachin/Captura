using System;
using System.Drawing;

namespace Captura.Models
{
    class MouseDragStep : KeyModifiedStep
    {
        readonly Point _start, _end;
        readonly MouseClickSettings _settings;

        public MouseDragStep(Point StartPoint,
            Point EndPoint,
            MouseClickSettings Settings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap) : base(KeystrokesSettings, Keymap)
        {
            _start = StartPoint;
            _end = EndPoint;
            _settings = Settings;
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var start = _start;
            var end = _end;

            if (PointTransform != null)
            {
                start = PointTransform(start);
                end = PointTransform(end);
            }

            Editor.DrawArrow(start, end, _settings.Color, _settings.Radius);

            base.Draw(Editor, PointTransform);
        }
    }
}
