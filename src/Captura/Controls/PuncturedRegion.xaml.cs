using System.Windows;

namespace Captura
{
    public partial class PuncturedRegion
    {
        public Rect? Region
        {
            get => (Rect?)GetValue(RegionProperty);
            set => SetValue(RegionProperty, value);
        }

        public static readonly DependencyProperty RegionProperty = DependencyProperty.Register(
            nameof(Region),
            typeof(Rect?),
            typeof(PuncturedRegion),
            new PropertyMetadata(RegionChanged));

        static void RegionChanged(DependencyObject Obj, DependencyPropertyChangedEventArgs E)
        {
             if (Obj is PuncturedRegion r)
             {
                switch (E.NewValue)
                {
                    case null:
                        // Must not be collapsed, since ActualWidth and ActualHeight are used.
                        r.Visibility = Visibility.Hidden;
                        break;

                    case Rect region:
                        var w = r.ActualWidth;
                        var h = r.ActualHeight;

                        r.BorderTop.Margin = new Thickness();
                        r.BorderTop.Width = w;
                        r.BorderTop.Height = region.Top.Clip(0, h);

                        r.BorderBottom.Margin = new Thickness(0, region.Bottom, 0, 0);
                        r.BorderBottom.Width = w;
                        r.BorderBottom.Height = (h - region.Bottom).Clip(0, h);

                        r.BorderLeft.Margin = new Thickness(0, region.Top, 0, 0);
                        r.BorderLeft.Width = region.Left.Clip(0, w);
                        r.BorderLeft.Height = region.Height;

                        r.BorderRight.Margin = new Thickness(region.Right, region.Top, 0, 0);
                        r.BorderRight.Width = (w - region.Right).Clip(0, w);
                        r.BorderRight.Height = region.Height;

                        r.Visibility = Visibility.Visible;
                        break;
                }
             }
        }

        public PuncturedRegion()
        {
            InitializeComponent();
        }
    }
}
