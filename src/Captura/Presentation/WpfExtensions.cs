using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Captura
{
    public static class WpfExtensions
    {
        public static void ShowAndFocus(this Window W)
        {
            W.Show();

            W.Activate();
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
                if (Element.RenderTransform == Transform.Identity)
                {
                    Element.RenderTransform = new TranslateTransform();
                }

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

                var storyboard = new Storyboard
                {
                    Children = { animation }
                };

                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.X"));

                storyboard.Begin(Element);
            });
        }
    }
}
