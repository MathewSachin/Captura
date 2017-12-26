using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Captura
{
    public class InkToolToIconConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var app = Application.Current;

            switch (Value)
            {
                case InkCanvasEditingMode.Ink:
                    return app.FindResource("Icon_Pencil");

                case InkCanvasEditingMode.EraseByPoint:
                    return app.FindResource("Icon_Eraser");

                case InkCanvasEditingMode.EraseByStroke:
                    return app.FindResource("Icon_StrokeEraser");

                case InkCanvasEditingMode.Select:
                    return app.FindResource("Icon_Select");
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Binding.DoNothing;
        }
    }
}