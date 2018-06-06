using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura
{
    public class CroppingAdorner : Adorner
    {
        readonly Thumb _bottomLeft, _bottomRight, _topLeft, _topRight;

        readonly VisualCollection _visualChildren;

        public CroppingAdorner(UIElement AdornedElement) : base(AdornedElement)
        {
            _visualChildren = new VisualCollection(this);

            BuildAdornerCorner(ref _topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref _topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref _bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref _bottomRight, Cursors.SizeNWSE);

            _bottomLeft.DragDelta += HandleBottomLeft;
            _bottomRight.DragDelta += HandleBottomRight;
            _topLeft.DragDelta += HandleTopLeft;
            _topRight.DragDelta += HandleTopRight;
        }

        void HandleTopRight(object Sender, DragDeltaEventArgs DeltaEventArgs)
        {
            if (AdornedElement is FrameworkElement adornedElement && Sender is Thumb hitThumb)
            {
                EnforceSize(adornedElement);
            }
        }

        void HandleTopLeft(object Sender, DragDeltaEventArgs DeltaEventArgs)
        {
            if (AdornedElement is FrameworkElement adornedElement && Sender is Thumb hitThumb)
            {
                EnforceSize(adornedElement);
            }
        }

        void HandleBottomRight(object Sender, DragDeltaEventArgs DeltaEventArgs)
        {
            if (AdornedElement is FrameworkElement adornedElement && Sender is Thumb hitThumb)
            {
                EnforceSize(adornedElement);
            }
        }

        void HandleBottomLeft(object Sender, DragDeltaEventArgs DeltaEventArgs)
        {
            if (AdornedElement is FrameworkElement adornedElement && Sender is Thumb hitThumb)
            {
                EnforceSize(adornedElement);
            }
        }

        void BuildAdornerCorner(ref Thumb CornerThumb, Cursor CustomizedCursor)
        {
            if (CornerThumb != null)
                return;

            CornerThumb = new Thumb
            {
                Cursor = CustomizedCursor,
                Height = 10,
                Width = 10,
                Opacity = 0.4,
                Background = new SolidColorBrush(Colors.MediumBlue)
            };

            _visualChildren.Add(CornerThumb);
        }

        protected override Size ArrangeOverride(Size FinalSize)
        {
            var desiredWidth = AdornedElement.DesiredSize.Width;
            var desiredHeight = AdornedElement.DesiredSize.Height;

            var adornerWidth = DesiredSize.Width;
            var adornerHeight = DesiredSize.Height;

            _topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            _topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            _bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            _bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            return FinalSize;
        }

        void EnforceSize(FrameworkElement Element)
        {
            if (Element.Width.Equals(double.NaN))
                Element.Width = Element.DesiredSize.Width;

            if (Element.Height.Equals(double.NaN))
                Element.Height = Element.DesiredSize.Height;

            if (Element.Parent is FrameworkElement parent)
            {
                Element.MaxHeight = parent.ActualHeight;
                Element.MaxWidth = parent.ActualWidth;
            }
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override Visual GetVisualChild(int Index) => _visualChildren[Index];
    }
}