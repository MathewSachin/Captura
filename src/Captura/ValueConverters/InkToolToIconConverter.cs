using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Captura
{
    public class InkToolToIconConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var app = Application.Current;

            switch (Value)
            {
                case InkCanvasEditingMode.Ink:
                case ExtendedInkTool.Pen:
                    return app.FindResource("IconPencil");

                case InkCanvasEditingMode.EraseByPoint:
                case ExtendedInkTool.Eraser:
                    return app.FindResource("IconEraser");

                case InkCanvasEditingMode.EraseByStroke:
                case ExtendedInkTool.StrokeEraser:
                    return app.FindResource("IconStrokeEraser");

                case InkCanvasEditingMode.Select:
                case ExtendedInkTool.Select:
                    return app.FindResource("IconSelect");

                case ExtendedInkTool.Line:
                    return app.FindResource("IconLine");

                case ExtendedInkTool.Rectangle:
                    return app.FindResource("IconRectangle");

                case ExtendedInkTool.Ellipse:
                    return app.FindResource("IconEllipse");
            }

            return app.FindResource("IconCursor");
        }
    }
}