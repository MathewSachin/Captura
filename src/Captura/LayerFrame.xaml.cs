using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public partial class LayerFrame
    {
        public LayerFrame()
        {
            InitializeComponent();

            Loaded += (S, E) =>
            {
                if (Parent is Canvas c)
                {
                    c.MouseMove += Canvas_OnMouseMove;
                    c.MouseUp += Canvas_OnMouseUp;
                }
            };
        }

        HitType _mouseHitType = HitType.None;

        Point _lastPoint;

        bool _isDragging;

        HitType SetHitType()
        {
            var point = Mouse.GetPosition(this);

            const int left = 0;
            const int top = 0;

            var right = left + ActualWidth;
            var bottom = top + ActualHeight;

            if (point.X < left || point.X > right || point.Y < top || point.Y > bottom)
            {
                return HitType.None;
            }

            const int gap = 10;

            // Left edge
            if (point.X - left < gap)
            {
                if (point.Y - top < gap)
                {
                    return HitType.UpperLeft;
                }

                if (bottom - point.Y < gap)
                {
                    return HitType.LowerLeft;
                }

                return HitType.Left;
            }

            // Right edge
            if (right - point.X < gap)
            {
                if (point.Y - top < gap)
                {
                    return HitType.UpperRight;
                }

                if (bottom - point.Y < gap)
                {
                    return HitType.LowerRight;
                }

                return HitType.Right;
            }

            if (point.Y - top < gap)
            {
                return HitType.Top;
            }

            if (bottom - point.Y < gap)
            {
                return HitType.Bottom;
            }

            return HitType.Body;
        }

        void SetMouseCursor()
        {
            var desired = Cursors.Arrow;

            switch (_mouseHitType)
            {
                case HitType.None:
                    desired = Cursors.Arrow;
                    break;

                case HitType.Body:
                    desired = Cursors.SizeAll;
                    break;

                case HitType.UpperLeft:
                case HitType.LowerRight:
                    desired = Cursors.SizeNWSE;
                    break;

                case HitType.LowerLeft:
                case HitType.UpperRight:
                    desired = Cursors.SizeNESW;
                    break;

                case HitType.Top:
                case HitType.Bottom:
                    desired = Cursors.SizeNS;
                    break;

                case HitType.Left:
                case HitType.Right:
                    desired = Cursors.SizeWE;
                    break;
            }

            if (Cursor != desired)
            {
                Cursor = desired;
            }
        }

        void LayerFrame_OnMouseDown(object Sender, MouseButtonEventArgs E)
        {
            _mouseHitType = SetHitType();

            SetMouseCursor();

            if (_mouseHitType == HitType.None)
                return;

            _lastPoint = Mouse.GetPosition(Parent as Canvas);
            _isDragging = true;
        }

        void Canvas_OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            _isDragging = false;
        }

        void Canvas_OnMouseMove(object Sender, MouseEventArgs E)
        {
            if (_isDragging)
            {
                var point = Mouse.GetPosition(Parent as Canvas);
                var offsetX = point.X - _lastPoint.X;
                var offsetY = point.Y - _lastPoint.Y;

                var newX = Canvas.GetLeft(this);
                var newY = Canvas.GetTop(this);
                var newWidth = ActualWidth;
                var newHeight = ActualHeight;

                switch (_mouseHitType)
                {
                    case HitType.Body:
                        newX += offsetX;
                        newY += offsetY;
                        break;

                    case HitType.UpperLeft:
                        newX += offsetX;
                        newY += offsetY;
                        newWidth -= offsetX;
                        newHeight -= offsetY;
                        break;

                    case HitType.UpperRight:
                        newY += offsetY;
                        newWidth += offsetX;
                        newHeight -= offsetY;
                        break;

                    case HitType.LowerRight:
                        newWidth += offsetX;
                        newHeight += offsetY;
                        break;

                    case HitType.LowerLeft:
                        newX += offsetX;
                        newWidth -= offsetX;
                        newHeight += offsetY;
                        break;

                    case HitType.Left:
                        newX += offsetX;
                        newWidth -= offsetX;
                        break;

                    case HitType.Right:
                        newWidth += offsetX;
                        break;

                    case HitType.Bottom:
                        newHeight += offsetY;
                        break;

                    case HitType.Top:
                        newY += offsetY;
                        newHeight -= offsetY;
                        break;
                }

                if (newWidth > 0 && newHeight > 0)
                {
                    Canvas.SetLeft(this, newX);
                    Canvas.SetTop(this, newY);

                    Width = newWidth;
                    Height = newHeight;

                    _lastPoint = point;
                }
            }
            else
            {
                _mouseHitType = SetHitType();

                SetMouseCursor();
            }
        }
    }
}