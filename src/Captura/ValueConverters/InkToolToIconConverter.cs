using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public class InkToolToIconConverter : OneWayConverter
    {
        string GetPath(object Value)
        {
            var icons = ServiceProvider.Get<IIconSet>();

            switch (Value)
            {
                case InkCanvasEditingMode.Ink:
                case ExtendedInkTool.Pen:
                    return icons.Pencil;

                case InkCanvasEditingMode.EraseByPoint:
                case ExtendedInkTool.Eraser:
                    return icons.Eraser;

                case InkCanvasEditingMode.EraseByStroke:
                case ExtendedInkTool.StrokeEraser:
                    return icons.StrokeEraser;

                case InkCanvasEditingMode.Select:
                case ExtendedInkTool.Select:
                    return icons.Select;

                case ExtendedInkTool.Line:
                    return icons.Line;

                case ExtendedInkTool.Rectangle:
                    return icons.Rectangle;

                case ExtendedInkTool.Ellipse:
                    return icons.Ellipse;

                case ExtendedInkTool.Arrow:
                    return icons.Arrow;
            }

            return icons.Cursor;
        }


        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Geometry.Parse(GetPath(Value));
        }
    }
}