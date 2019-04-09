using System;
using System.Drawing;

namespace Captura.Models
{
    class MouseDragStep : IRecordStep
    {
        readonly Point _start, _end;
        readonly MouseClickSettings _settings;

        public MouseDragStep(Point StartPoint, Point EndPoint, MouseClickSettings Settings)
        {
            _start = StartPoint;
            _end = EndPoint;
            _settings = Settings;
        }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var start = _start;
            var end = _end;

            if (PointTransform != null)
            {
                start = PointTransform(start);
                end = PointTransform(end);
            }

            Editor.DrawArrow(start, end, _settings.Color, _settings.Radius);
        }

        public bool Merge(IRecordStep NextStep) => false;
    }
}
