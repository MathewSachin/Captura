using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    class ScrollStep : IRecordStep
    {
        public MouseEventArgs Args { get; }

        public ScrollStep(MouseEventArgs Args)
        {
            this.Args = Args;
        }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var p = Args.Location;

            Editor.FillEllipse(Color.Yellow, new RectangleF(p.X - 10, p.Y - 20, 20, 40));
        }

        public bool Merge(IRecordStep NextStep)
        {
            if (NextStep is ScrollStep nextStep)
            {
                // Scroll in same direction
                if (Math.Sign(Args.Delta) == Math.Sign(nextStep.Args.Delta))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
