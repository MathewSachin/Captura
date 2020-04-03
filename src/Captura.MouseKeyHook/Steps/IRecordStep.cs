using System;
using System.Drawing;

namespace Captura.MouseKeyHook.Steps
{
    interface IRecordStep
    {
        void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform);

        bool Merge(IRecordStep NextStep);
    }
}
