using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public class InkToolToIconConverter : OneWayConverter
    {
        static string GetPath(object Value)
        {
            var icons = ServiceProvider.Get<IIconSet>();

            switch (Value)
            {
                case InkCanvasEditingMode.Ink:
                    return icons.Pencil;

                case InkCanvasEditingMode.EraseByPoint:
                    return icons.Eraser;

                case InkCanvasEditingMode.EraseByStroke:
                    return icons.StrokeEraser;

                case InkCanvasEditingMode.Select:
                    return icons.Select;
            }

            return icons.Cursor;
        }

        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Geometry.Parse(GetPath(Value));
        }
    }
}