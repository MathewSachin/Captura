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
                    return app.FindResource("Icon_Pencil");

                case InkCanvasEditingMode.EraseByPoint:
                case ExtendedInkTool.Eraser:
                    return app.FindResource("Icon_Eraser");

                case InkCanvasEditingMode.EraseByStroke:
                case ExtendedInkTool.StrokeEraser:
                    return app.FindResource("Icon_StrokeEraser");

                case InkCanvasEditingMode.Select:
                case ExtendedInkTool.Select:
                    return app.FindResource("Icon_Select");

                case ExtendedInkTool.Line:
                    return app.FindResource("Icon_Line");

                case ExtendedInkTool.Rectangle:
                    return app.FindResource("Icon_Rectangle");

                case ExtendedInkTool.Ellipse:
                    return app.FindResource("Icon_Ellipse");
            }

            return app.FindResource("Icon_Cursor");
        }
    }
}