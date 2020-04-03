using System;
using System.Drawing;

namespace Captura.MouseKeyHook.Steps
{
    class MouseDragStep : KeyModifiedStep
    {
        readonly MouseClickSettings _settings;

        public Point StartPoint { get; set; }
        public Point EndPoint { get; }

        public MouseDragStep(Point EndPoint,
            MouseClickSettings Settings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap) : base(KeystrokesSettings, Keymap)
        {
            this.EndPoint = EndPoint;
            _settings = Settings;
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var start = StartPoint;
            var end = EndPoint;

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
