using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DColor = System.Drawing.Color;

namespace Captura
{
    public static class WpfExtensions
    {
        public static void ShowAndFocus(this Window W)
        {
            if (W.IsVisible && W.WindowState == WindowState.Minimized)
            {
                W.WindowState = WindowState.Normal;
            }

            W.Show();

            W.Activate();
        }

        public static DColor ToDrawingColor(this Color C)
        {
            return DColor.FromArgb(C.A, C.R, C.G, C.B);
        }

        public static Color ToWpfColor(this DColor C)
        {
            return Color.FromArgb(C.A, C.R, C.G, C.B);
        }

        public static Color ParseColor(string S)
        {
            if (ColorConverter.ConvertFromString(S) is Color c)
                return c;

            return Colors.Transparent;
        }

        public static void Shake(this FrameworkElement Element)
        {
            Element.Dispatcher.Invoke(() =>
            {
                var transform = new TranslateTransform();
                Element.RenderTransform = transform;

                const int delta = 5;

                var animation = new DoubleAnimationUsingKeyFrames
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(1),
                    Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                    KeyFrames =
                    {
                        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0)),
                        new EasingDoubleKeyFrame(delta, KeyTime.FromPercent(0.25)),
                        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0.5)),
                        new EasingDoubleKeyFrame(-delta, KeyTime.FromPercent(0.75)),
                        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1)),
                    }
                };

                transform.BeginAnimation(TranslateTransform.XProperty, animation);
            });
        }
    }
}
