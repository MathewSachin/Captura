using System;
using System.Windows;
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
                if (Parent is UIElement element)
                {
                    element.MouseDown += Parent_OnMouseDown;
                    element.MouseMove += Parent_OnMouseMove;
                    element.MouseUp += Parent_OnMouseUp;
                }
            };
        }

        public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register(
            "CanResize", typeof(bool), typeof(LayerFrame), new PropertyMetadata(true));

        public bool CanResize
        {
            get => (bool) GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
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

            if (!CanResize)
                return HitType.Body;

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

        void Parent_OnMouseDown(object Sender, MouseButtonEventArgs E)
        {
            _mouseHitType = SetHitType();

            SetMouseCursor();

            if (_mouseHitType == HitType.None)
                return;

            _lastPoint = Mouse.GetPosition(Parent as IInputElement);
            _isDragging = true;
        }

        void Parent_OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            _isDragging = false;
        }

        void Parent_OnMouseMove(object Sender, MouseEventArgs E)
        {
            if (_isDragging)
            {
                var point = Mouse.GetPosition(Parent as IInputElement);
                var offsetX = point.X - _lastPoint.X;
                var offsetY = point.Y - _lastPoint.Y;

                var newX = HorizontalAlignment == HorizontalAlignment.Right ? Margin.Right : Margin.Left;
                var newY = VerticalAlignment == VerticalAlignment.Bottom ? Margin.Bottom : Margin.Top;
                var newWidth = ActualWidth;
                var newHeight = ActualHeight;

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

                switch (_mouseHitType)
                {
                    case HitType.Body:
                        ModifyX(HorizontalAlignment != HorizontalAlignment.Right);
                        ModifyY(VerticalAlignment != VerticalAlignment.Bottom);
                        break;

                    case HitType.UpperLeft:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyWidth(false);
                        }
                        else
                        {
                            ModifyX(true);
                            ModifyWidth(false);
                        }

                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyHeight(false);
                        }
                        else
                        {
                            ModifyY(true);
                            ModifyHeight(false);
                        }
                        break;

                    case HitType.UpperRight:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyX(false);
                            ModifyWidth(true);
                        }
                        else ModifyWidth(true);

                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyHeight(false);
                        }
                        else
                        {
                            ModifyY(true);
                            ModifyHeight(false);
                        }
                        break;

                    case HitType.LowerRight:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyX(false);
                            ModifyWidth(true);
                        }
                        else ModifyWidth(true);

                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyY(false);
                            ModifyHeight(true);
                        }
                        else ModifyHeight(true);
                        break;

                    case HitType.LowerLeft:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyWidth(false);
                        }
                        else
                        {
                            ModifyX(true);
                            ModifyWidth(false);
                        }

                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyY(false);
                            ModifyHeight(true);
                        }
                        else ModifyHeight(true);
                        break;

                    case HitType.Left:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyWidth(false);
                        }
                        else
                        {
                            ModifyX(true);
                            ModifyWidth(false);
                        }
                        break;

                    case HitType.Right:
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ModifyX(false);
                            ModifyWidth(true);
                        }
                        else ModifyWidth(true);
                        break;

                    case HitType.Bottom:
                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyY(false);
                            ModifyHeight(true);
                        }
                        else ModifyHeight(true);
                        break;

                    case HitType.Top:
                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            ModifyHeight(false);
                        }
                        else
                        {
                            ModifyY(true);
                            ModifyHeight(false);
                        }
                        break;
                }

                if (newWidth > 0 && newHeight > 0)
                {
                    double left = 0, top = 0, right = 0, bottom = 0;

                    if (HorizontalAlignment == HorizontalAlignment.Right)
                        right = newX;
                    else left = newX;

                    if (VerticalAlignment == VerticalAlignment.Bottom)
                        bottom = newY;
                    else top = newY;

                    Margin = new Thickness(left, top, right, bottom);

                    PositionUpdated?.Invoke(newX, newY, newWidth, newHeight);

                    if (_mouseHitType != HitType.Body)
                    {
                        Width = newWidth;
                        Height = newHeight;
                    }

                    _lastPoint = point;
                }
            }
            else
            {
                _mouseHitType = SetHitType();

                SetMouseCursor();
            }
        }

        public event Action<double, double, double, double> PositionUpdated;
    }
}