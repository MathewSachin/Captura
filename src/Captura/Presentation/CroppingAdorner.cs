using System;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Point = System.Drawing.Point;

namespace Captura
{
    public class CroppingAdorner : Adorner
    {
        #region Private variables
        // Width of the thumbs. I know these really aren't "pixels", but px is still a good mnemonic.
        const int CpxThumbWidth = 6;

        // PuncturedRect to hold the "Cropping" portion of the adorner
        readonly PuncturedRect _prCropMask;

        // Canvas to hold the thumbs so they can be moved in response to the user
        readonly Canvas _cnvThumbs;

        // Cropping adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        readonly CropThumb _crtTopLeft;

        readonly CropThumb _crtTopRight;
        readonly CropThumb _crtBottomLeft;
        readonly CropThumb _crtBottomRight;

        readonly CropThumb _crtTop;
        readonly CropThumb _crtLeft;
        readonly CropThumb _crtBottom;
        readonly CropThumb _crtRight;

        readonly CropThumb _crtMove;

        // To store and manage the adorner's visual children.
        readonly VisualCollection _vc;
        #endregion
        
        #region Routed Events
        public static readonly RoutedEvent CropChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(CropChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CroppingAdorner));

        public event RoutedEventHandler CropChanged
        {
            add => AddHandler(CropChangedEvent, value);
            remove => RemoveHandler(CropChangedEvent, value);
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty FillProperty = Shape.FillProperty.AddOwner(typeof(CroppingAdorner));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        static void FillPropChanged(DependencyObject D, DependencyPropertyChangedEventArgs Args)
        {
            if (D is CroppingAdorner crp)
            {
                crp._prCropMask.Fill = (Brush)Args.NewValue;
            }
        }
        #endregion

        #region Constructor
        static CroppingAdorner()
        {
            var clr = Colors.Red;
            
            clr.A = 80;
            FillProperty.OverrideMetadata(typeof(CroppingAdorner),
                new PropertyMetadata(
                    new SolidColorBrush(clr),
                    FillPropChanged));
        }

        public CroppingAdorner(UIElement AdornedElement, Rect rcInit)
            : base(AdornedElement)
        {
            _vc = new VisualCollection(this);
            _prCropMask = new PuncturedRect
            {
                IsHitTestVisible = false,
                RectInterior = rcInit,
                Fill = Fill
            };
            _vc.Add(_prCropMask);
            _cnvThumbs = new Canvas
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            _vc.Add(_cnvThumbs);
            BuildCorner(ref _crtTop, Cursors.SizeNS);
            BuildCorner(ref _crtBottom, Cursors.SizeNS);
            BuildCorner(ref _crtLeft, Cursors.SizeWE);
            BuildCorner(ref _crtRight, Cursors.SizeWE);
            BuildCorner(ref _crtTopLeft, Cursors.SizeNWSE);
            BuildCorner(ref _crtTopRight, Cursors.SizeNESW);
            BuildCorner(ref _crtBottomLeft, Cursors.SizeNESW);
            BuildCorner(ref _crtBottomRight, Cursors.SizeNWSE);

            BuildCorner(ref _crtMove, Cursors.SizeAll);

            // Add handlers for Cropping.
            _crtBottomLeft.DragDelta += HandleBottomLeft;
            _crtBottomRight.DragDelta += HandleBottomRight;
            _crtTopLeft.DragDelta += HandleTopLeft;
            _crtTopRight.DragDelta += HandleTopRight;
            _crtTop.DragDelta += HandleTop;
            _crtBottom.DragDelta += HandleBottom;
            _crtRight.DragDelta += HandleRight;
            _crtLeft.DragDelta += HandleLeft;

            _crtMove.DragDelta += HandleMove;

            // We have to keep the clipping interior withing the bounds of the adorned element
            // so we have to track it's size to guarantee that...

            if (AdornedElement is FrameworkElement fel)
            {
                fel.SizeChanged += AdornedElement_SizeChanged;
            }
        }
        #endregion

        #region Thumb handlers
        // Generic handler for Cropping
        void HandleThumb(
            double drcL,
            double drcT,
            double drcW,
            double drcH,
            double dx,
            double dy)
        {
            var rcInterior = _prCropMask.RectInterior;

            if (rcInterior.Width + drcW * dx < 0)
            {
                dx = -rcInterior.Width / drcW;
            }

            if (rcInterior.Height + drcH * dy < 0)
            {
                dy = -rcInterior.Height / drcH;
            }

            rcInterior = new Rect(
                rcInterior.Left + drcL * dx,
                rcInterior.Top + drcT * dy,
                rcInterior.Width + drcW * dx,
                rcInterior.Height + drcH * dy);

            _prCropMask.RectInterior = rcInterior;
            SetThumbs(_prCropMask.RectInterior);
            RaiseEvent(new RoutedEventArgs(CropChangedEvent, this));
        }

        void HandleMove(object Sender, DragDeltaEventArgs Args)
        {
            if (AdornedElement is FrameworkElement fel)
            {
                var rcInterior = _prCropMask.RectInterior;

                var left = rcInterior.Left + Args.HorizontalChange;
                var top = rcInterior.Top + Args.VerticalChange;

                if (left < 0)
                    left = 0;

                if (left + rcInterior.Width > fel.ActualWidth)
                    left = fel.ActualWidth - rcInterior.Width;

                if (top < 0)
                    top = 0;

                if (top + rcInterior.Height > fel.ActualHeight)
                    top = fel.ActualHeight - rcInterior.Height;

                rcInterior = new Rect(left, top, rcInterior.Width, rcInterior.Height);

                _prCropMask.RectInterior = rcInterior;

                SetThumbs(_prCropMask.RectInterior);

                RaiseEvent(new RoutedEventArgs(CropChangedEvent, this));
            }
        }

        // Handler for Cropping from the bottom-left.
        void HandleBottomLeft(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    1, 0, -1, 1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom-right.
        void HandleBottomRight(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 1, 1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-right.
        void HandleTopRight(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    0, 1, 1, -1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-left.
        void HandleTopLeft(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    1, 1, -1, -1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the top.
        void HandleTop(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    0, 1, 0, -1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the left.
        void HandleLeft(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    1, 0, -1, 0,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the right.
        void HandleRight(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 1, 0,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom.
        void HandleBottom(object Sender, DragDeltaEventArgs Args)
        {
            if (Sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 0, 1,
                    Args.HorizontalChange,
                    Args.VerticalChange);
            }
        }
        #endregion
        
        void AdornedElement_SizeChanged(object Sender, SizeChangedEventArgs E)
        {
            var ratio = E.NewSize.Width / E.PreviousSize.Width;

            var rcInterior = _prCropMask.RectInterior;
            
            double intLeft = rcInterior.Left * ratio,
                intTop = rcInterior.Top * ratio,
                intWidth = rcInterior.Width * ratio,
                intHeight = rcInterior.Height * ratio;
            
            _prCropMask.RectInterior = new Rect(intLeft, intTop, intWidth, intHeight);
        }
        
        #region Arranging/positioning
        void SetThumbs(Rect rc)
        {
            _crtBottomRight.SetPos(rc.Right, rc.Bottom);
            _crtTopLeft.SetPos(rc.Left, rc.Top);
            _crtTopRight.SetPos(rc.Right, rc.Top);
            _crtBottomLeft.SetPos(rc.Left, rc.Bottom);
            _crtTop.SetPos(rc.Left + rc.Width / 2, rc.Top);
            _crtBottom.SetPos(rc.Left + rc.Width / 2, rc.Bottom);
            _crtLeft.SetPos(rc.Left, rc.Top + rc.Height / 2);
            _crtRight.SetPos(rc.Right, rc.Top + rc.Height / 2);

            _crtMove.SetPos(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size FinalSize)
        {
            var rcExterior = new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            _prCropMask.RectExterior = rcExterior;
            var rcInterior = _prCropMask.RectInterior;
            _prCropMask.Arrange(rcExterior);

            SetThumbs(rcInterior);
            _cnvThumbs.Arrange(rcExterior);
            return FinalSize;
        }
        #endregion
        
        public BitmapSource BpsCrop(BitmapSource Bmp)
        {
            var ratio = Bmp.PixelWidth / AdornedElement.RenderSize.Width;

            var rcInterior = _prCropMask.RectInterior;

            Point ToPoint(double X, double Y)
            {
                return new Point((int)(X * ratio), (int)(Y * ratio));
            }

            var pxFromSize = ToPoint(rcInterior.Width, rcInterior.Height);
            
            var pxFromPos = ToPoint(rcInterior.Left, rcInterior.Top);
            var pxWhole = ToPoint(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);

            pxFromSize.X = Math.Max(Math.Min(pxWhole.X - pxFromPos.X, pxFromSize.X), 0);
            pxFromSize.Y = Math.Max(Math.Min(pxWhole.Y - pxFromPos.Y, pxFromSize.Y), 0);

            if (pxFromSize.X == 0 || pxFromSize.Y == 0)
            {
                return null;
            }

            var rcFrom = new Int32Rect(pxFromPos.X, pxFromPos.Y, pxFromSize.X, pxFromSize.Y);

            return new CroppedBitmap(Bmp, rcFrom);
        }
        
        void BuildCorner(ref CropThumb crt, Cursor crs)
        {
            if (crt != null)
                return;

            crt = new CropThumb(CpxThumbWidth)
            {
                Cursor = crs
            };

            _cnvThumbs.Children.Add(crt);
        }

        #region Visual tree overrides
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount => _vc.Count;

        protected override Visual GetVisualChild(int Index) => _vc[Index];
        #endregion
        
        class CropThumb : Thumb
        {
            readonly int _cpx;

            public CropThumb(int cpx)
            {
                _cpx = cpx;
            }
            
            protected override Visual GetVisualChild(int Index) => null;

            protected override void OnRender(DrawingContext DrawingContext)
            {
                DrawingContext.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), new Rect(new Size(_cpx, _cpx)), 1, 1);
            }

            public void SetPos(double X, double Y)
            {
                Canvas.SetTop(this, Y - _cpx / 2.0);
                Canvas.SetLeft(this, X - _cpx / 2.0);
            }
        }
    }
}