using System;
using System.Drawing;

namespace Captura.MouseKeyHook.Steps
{
    class MouseDragBeginStep : KeyModifiedStep
    {
        readonly Point _start;
        Point _end;
        readonly MouseClickSettings _settings;

        const int DragBeginLength = 50;

        public MouseDragBeginStep(Point StartPoint,
            MouseClickSettings Settings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap) : base(KeystrokesSettings, Keymap)
        {
            _start = StartPoint;
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

        public override bool Merge(IRecordStep NextStep)
        {
            if (NextStep is MouseDragStep mouseDragStep)
            {
                mouseDragStep.StartPoint = _start;
                _end = mouseDragStep.EndPoint;

                var diffX = (_end.X - _start.X);
                var diffY = (_end.Y - _start.Y);

                var mag = Math.Sqrt(diffX * diffX + diffY * diffY);

                _end = new Point((int)(_start.X + diffX * DragBeginLength / mag),
                    (int)(_start.Y + diffY * DragBeginLength / mag));

                return false;
            }
            else if (NextStep is MouseClickStep)
            {
                return true;
            }

            return base.Merge(NextStep);
        }
    }
}
