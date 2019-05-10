using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura
{
    public class OverlayPositionAdorner : Adorner
    {
        #region Thumbs
        readonly Thumb _topLeft;
        readonly Thumb _topRight;
        readonly Thumb _bottomLeft;
        readonly Thumb _bottomRight;

        readonly Thumb _top;
        readonly Thumb _left;
        readonly Thumb _right;
        readonly Thumb _bottom;

        readonly Thumb _center;
        #endregion

        readonly bool _canResize;

        readonly VisualCollection _visualChildren;

        public OverlayPositionAdorner(UIElement Element, bool CanResize = true) : base(Element)
        {
            _canResize = CanResize;

            _visualChildren = new VisualCollection(this);

            BuildAdornerThumb(ref _center, Cursors.Hand);
            _center.Opacity = 0;

            _center.DragDelta += (S, E) => HandleDrag(HitType.Body, E);

            if (CanResize)
            {
                BuildAdornerThumb(ref _topLeft, Cursors.SizeNWSE);
                BuildAdornerThumb(ref _topRight, Cursors.SizeNESW);
                BuildAdornerThumb(ref _bottomLeft, Cursors.SizeNESW);
                BuildAdornerThumb(ref _bottomRight, Cursors.SizeNWSE);

                BuildAdornerThumb(ref _top, Cursors.SizeNS);
                BuildAdornerThumb(ref _left, Cursors.SizeWE);
                BuildAdornerThumb(ref _right, Cursors.SizeWE);
                BuildAdornerThumb(ref _bottom, Cursors.SizeNS);

                _topLeft.DragDelta += (S, E) => HandleDrag(HitType.UpperLeft, E);
                _topRight.DragDelta += (S, E) => HandleDrag(HitType.UpperRight, E);
                _bottomLeft.DragDelta += (S, E) => HandleDrag(HitType.LowerLeft, E);
                _bottomRight.DragDelta += (S, E) => HandleDrag(HitType.LowerRight, E);

                _top.DragDelta += (S, E) => HandleDrag(HitType.Top, E);
                _left.DragDelta += (S, E) => HandleDrag(HitType.Left, E);
                _right.DragDelta += (S, E) => HandleDrag(HitType.Right, E);
                _bottom.DragDelta += (S, E) => HandleDrag(HitType.Bottom, E);
            }

            Opacity = 0.01;

            MouseEnter += (S, E) => Opacity = 1;
            MouseLeave += (S, E) => Opacity = 0.01;
        }

        void HandleDrag(HitType MouseHitType, DragDeltaEventArgs Args)
        {
            if (!(AdornedElement is FrameworkElement fel))
                return;

            var offsetX = (int) Args.HorizontalChange;
            var offsetY = (int) Args.VerticalChange;

            var newX = (int) fel.Margin.Left;
            var newY = (int) fel.Margin.Top;
            var newWidth = (int) fel.ActualWidth;
            var newHeight = (int) fel.ActualHeight;

            void ModifyX(bool Possitive)
            {
                if (Possitive)
                    newX += offsetX;
                else newX -= offsetX;
            }

            void ModifyY(bool Possitive)
            {
                if (Possitive)
                    newY += offsetY;
                else newY -= offsetY;
            }

            void ModifyWidth(bool Possitive)
            {
                if (Possitive)
                    newWidth += offsetX;
                else newWidth -= offsetX;
            }

            void ModifyHeight(bool Possitive)
            {
                if (Possitive)
                    newHeight += offsetY;
                else newHeight -= offsetY;
            }

            switch (MouseHitType)
            {
                case HitType.Body:
                    ModifyX(true);
                    ModifyY(true);
                    break;

                case HitType.UpperLeft:                    
                    ModifyX(true);
                    ModifyWidth(false);
                    ModifyY(true);
                    ModifyHeight(false);
                    break;

                case HitType.UpperRight:
                    ModifyWidth(true);
                    ModifyY(true);
                    ModifyHeight(false);
                    break;

                case HitType.LowerRight:
                    ModifyWidth(true);
                    ModifyHeight(true);
                    break;

                case HitType.LowerLeft:
                    ModifyX(true);
                    ModifyWidth(false);
                    ModifyHeight(true);
                    break;

                case HitType.Left:
                    ModifyX(true);
                    ModifyWidth(false);
                    break;

                case HitType.Right:
                    ModifyWidth(true);
                    break;

                case HitType.Bottom:
                    ModifyHeight(true);
                    break;

                case HitType.Top:
                    ModifyHeight(false);
                    ModifyY(true);
                    ModifyHeight(false);
                    break;
            }

            if (newWidth > 0 && newHeight > 0)
            {
                if (newX < 0)
                {
                    newX = 0;
                }

                if (newY < 0)
                {
                    newY = 0;
                }

                var left = newX;
                var top = newY;

                fel.Margin = new Thickness(left, top, 0, 0);

                if (MouseHitType != HitType.Body)
                {
                    fel.Width = newWidth;
                    fel.Height = newHeight;
                }

                fel.Margin = new Thickness(left, top, 0, 0);

                PositionUpdated?.Invoke(new Rect(newX, newY, newWidth, newHeight));
            }
        }

        public event Action<Rect> PositionUpdated;

        void BuildAdornerThumb(ref Thumb CornerThumb, Cursor CustomizedCursors)
        {
            if (CornerThumb != null)
                return;

            CornerThumb = new Thumb
            {
                Cursor = CustomizedCursors,
                Height = 10,
                Width = 10,
                Opacity = 0.5,
                Background = new SolidColorBrush(Colors.Red)
            };

            _visualChildren.Add(CornerThumb);
        }

        protected override Size ArrangeOverride(Size FinalSize)
        {
            base.ArrangeOverride(FinalSize);

            var desireWidth = AdornedElement.RenderSize.Width;
            var desireHeight = AdornedElement.RenderSize.Height;

            var adornerWidth = DesiredSize.Width;
            var adornerHeight = DesiredSize.Height;

            _center.Height = desireHeight;
            _center.Width = desireWidth;
            _center.Arrange(new Rect(0, 0, desireWidth, desireHeight));

            if (_canResize)
            {
                _topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                _topRight.Arrange(new Rect(desireWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth,
                    adornerHeight));
                _bottomLeft.Arrange(new Rect(-adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth,
                    adornerHeight));
                _bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2,
                    adornerWidth, adornerHeight));

                _top.Arrange(new Rect(desireWidth / 2 - adornerWidth / 2, -adornerHeight / 2, adornerWidth,
                    adornerHeight));
                _left.Arrange(new Rect(-adornerWidth / 2, desireHeight / 2 - adornerHeight / 2, adornerWidth,
                    adornerHeight));
                _right.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight / 2 - adornerHeight / 2,
                    adornerWidth, adornerHeight));
                _bottom.Arrange(new Rect(desireWidth / 2 - adornerWidth / 2, desireHeight - adornerHeight / 2,
                    adornerWidth, adornerHeight));
            }

            return FinalSize;
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override Visual GetVisualChild(int Index) => _visualChildren[Index];
    }
}