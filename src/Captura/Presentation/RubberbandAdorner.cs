using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Captura
{
    public class RubberbandAdorner : Adorner
    {
        readonly UIElement _adornedElement;
        readonly RectangleGeometry _geometry;
        Point _anchorPoint;
        Rect _selectRect;
        
        public RubberbandAdorner(UIElement AdornedElement) : base(AdornedElement)
        {
            _adornedElement = AdornedElement;
            _selectRect = new Rect();
            _geometry = new RectangleGeometry();

            Rubberband = new Path
            {
                Data = _geometry,
                StrokeThickness = 2,
                Stroke = Brushes.Yellow,
                Opacity = 0.6,
                Visibility = Visibility.Hidden
            };

            AddVisualChild(Rubberband);

            MouseMove += DrawSelection;
            MouseUp += EndSelection;
        }

        public Rect SelectRect => _selectRect;
        public Path Rubberband { get; }
        protected override int VisualChildrenCount => 1;
        
        protected override Size ArrangeOverride(Size Size)
        {
            var finalSize = base.ArrangeOverride(Size);
            ((UIElement)GetVisualChild(0))?.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        public void StartSelection(Point AnchorPoint)
        {
            _anchorPoint = AnchorPoint;
            _selectRect.Size = new Size(10, 10);
            _selectRect.Location = _anchorPoint;
            _geometry.Rect = _selectRect;
            
            Rubberband.Visibility = Visibility.Visible;
        }

        void DrawSelection(object Sender, MouseEventArgs E)
        {
            if (E.LeftButton == MouseButtonState.Pressed)
            {
                var mousePosition = E.GetPosition(_adornedElement);
                _selectRect.X = mousePosition.X < _anchorPoint.X ? mousePosition.X : _anchorPoint.X;
                _selectRect.Y = mousePosition.Y < _anchorPoint.Y ? mousePosition.Y : _anchorPoint.Y;
                _selectRect.Width = Math.Abs(mousePosition.X - _anchorPoint.X);
                _selectRect.Height = Math.Abs(mousePosition.Y - _anchorPoint.Y);
                _geometry.Rect = _selectRect;
                var layer = AdornerLayer.GetAdornerLayer(_adornedElement);
                layer.InvalidateArrange();
            }
        }

        void EndSelection(object Sender, MouseButtonEventArgs E)
        {
            if (3 >= _selectRect.Width || 3 >= _selectRect.Height)
                Rubberband.Visibility = Visibility.Hidden;
            else EnableCrop?.Invoke();

            ReleaseMouseCapture();
        }

        public event Action EnableCrop;

        protected override Visual GetVisualChild(int Index) => Rubberband;
    }
}