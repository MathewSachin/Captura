using System;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public class PuncturedRect : Shape
    {
        public static readonly DependencyProperty RectInteriorProperty =
            DependencyProperty.Register(
                nameof(RectInterior),
                typeof(Rect),
                typeof(PuncturedRect),
                new FrameworkPropertyMetadata(
                    new Rect(0, 0, 0, 0),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,
                    CoerceRectInterior
                ),
                null
            );

        static object CoerceRectInterior(DependencyObject D, object Value)
        {
            if (D is PuncturedRect pr && Value is Rect rcProposed)
            {
                var rcExterior = pr.RectExterior;

                var left = Math.Max(rcProposed.Left, rcExterior.Left);
                var top = Math.Max(rcProposed.Top, rcExterior.Top);
                var width = Math.Min(rcProposed.Right, rcExterior.Right) - left;
                var height = Math.Min(rcProposed.Bottom, rcExterior.Bottom) - top;

                return new Rect(left, top, width, height);
            }

            return Value;
        }

        public Rect RectInterior
        {
            get => (Rect)GetValue(RectInteriorProperty);
            set => SetValue(RectInteriorProperty, value);
        }
        
        public static readonly DependencyProperty RectExteriorProperty =
            DependencyProperty.Register(
                nameof(RectExterior),
                typeof(Rect),
                typeof(PuncturedRect),
                new FrameworkPropertyMetadata(
                    new Rect(0, 0, double.MaxValue, double.MaxValue),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender
                ),
                null
            );

        public Rect RectExterior
        {
            get => (Rect)GetValue(RectExteriorProperty);
            set => SetValue(RectExteriorProperty, value);
        }
        
        public PuncturedRect() : this(new Rect(0, 0, double.MaxValue, double.MaxValue), new Rect()) { }

        public PuncturedRect(Rect RectExterior, Rect RectInterior)
        {
            this.RectInterior = RectInterior;
            this.RectExterior = RectExterior;
        }
        
        protected override Geometry DefiningGeometry
        {
            get
            {
                var pthgExt = new PathGeometry();
                var pthfExt = new PathFigure {StartPoint = RectExterior.TopLeft};
                pthfExt.Segments.Add(new LineSegment(RectExterior.TopRight, false));
                pthfExt.Segments.Add(new LineSegment(RectExterior.BottomRight, false));
                pthfExt.Segments.Add(new LineSegment(RectExterior.BottomLeft, false));
                pthfExt.Segments.Add(new LineSegment(RectExterior.TopLeft, false));
                pthgExt.Figures.Add(pthfExt);

                var rectIntSect = Rect.Intersect(RectExterior, RectInterior);
                var pthgInt = new PathGeometry();
                var pthfInt = new PathFigure {StartPoint = rectIntSect.TopLeft};
                pthfInt.Segments.Add(new LineSegment(rectIntSect.TopRight, false));
                pthfInt.Segments.Add(new LineSegment(rectIntSect.BottomRight, false));
                pthfInt.Segments.Add(new LineSegment(rectIntSect.BottomLeft, false));
                pthfInt.Segments.Add(new LineSegment(rectIntSect.TopLeft, false));
                pthgInt.Figures.Add(pthfInt);

                return new CombinedGeometry(GeometryCombineMode.Exclude, pthgExt, pthgInt);
            }
        }
    }
}
