using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.MouseKeyHook.Steps
{
    class ScrollStep : KeyModifiedStep
    {
        public MouseEventArgs Args { get; }

        readonly MouseClickSettings _settings;

        public ScrollStep(MouseEventArgs Args,
            MouseClickSettings Settings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap) : base(KeystrokesSettings, Keymap)
        {
            this.Args = Args;

            _settings = Settings;
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var p = Args.Location;

            var r = _settings.Radius;
            var d = 2 * r;

            Editor.FillEllipse(_settings.Color, new RectangleF(p.X - r, p.Y - r, d, d));

            var above = new Point(p.X, p.Y + r / 2);
            var below = new Point(p.X, p.Y - r / 2);

            if (Args.Delta < 0)
            {
                (above, below) = (below, above);
            }

            Editor.DrawArrow(above, below, _settings.BorderColor, r / 4f);

            base.Draw(Editor, PointTransform);
        }

        public override bool Merge(IRecordStep NextStep)
        {
            if (NextStep is ScrollStep nextStep)
            {
                // Scroll in same direction
                if (Math.Sign(Args.Delta) == Math.Sign(nextStep.Args.Delta))
                {
                    return true;
                }
            }

            return base.Merge(NextStep);
        }
    }
}
