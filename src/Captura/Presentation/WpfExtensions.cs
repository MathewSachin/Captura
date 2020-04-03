using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Captura.Video;
using Captura.ViewModels;
using Microsoft.Win32;
using Reactive.Bindings;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
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

        public static Rectangle ApplyDpi(this RectangleF Rectangle)
        {
            return new Rectangle((int)(Rectangle.Left * Dpi.X),
                (int)(Rectangle.Top * Dpi.Y),
                (int)(Rectangle.Width * Dpi.X),
                (int)(Rectangle.Height * Dpi.Y));
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
                        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1))
                    }
                };

                transform.BeginAnimation(TranslateTransform.XProperty, animation);
            });
        }

        public static bool SaveToPickedFile(this BitmapSource Bitmap, string DefaultFileName = null)
        {
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp|TIFF Image|*.tiff"
            };

            if (DefaultFileName != null)
            {
                sfd.FileName = Path.GetFileNameWithoutExtension(DefaultFileName);

                var dir = Path.GetDirectoryName(DefaultFileName);

                if (dir != null)
                {
                    sfd.InitialDirectory = dir;
                }
            }
            else sfd.FileName = "Untitled";

            if (!sfd.ShowDialog().GetValueOrDefault())
                return false;

            BitmapEncoder encoder;

            // Filter Index starts from 1
            switch (sfd.FilterIndex)
            {
                case 2:
                    encoder = new JpegBitmapEncoder();
                    break;

                case 3:
                    encoder = new BmpBitmapEncoder();
                    break;

                case 4:
                    encoder = new TiffBitmapEncoder();
                    break;

                default:
                    encoder = new PngBitmapEncoder();
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(Bitmap));

            using (var stream = sfd.OpenFile())
            {
                encoder.Save(stream);
            }

            return true;
        }

        public static void Bind(this FrameworkElement Control, DependencyProperty DependencyProperty, IReactiveProperty Property)
        {
            Control.SetBinding(DependencyProperty,
                new Binding(nameof(Property.Value))
                {
                    Source = Property,
                    Mode = BindingMode.TwoWay
                });
        }

        public static void BindOne<T>(this FrameworkElement Control, DependencyProperty DependencyProperty, IReadOnlyReactiveProperty<T> Property)
        {
            Control.SetBinding(DependencyProperty,
                new Binding(nameof(Property.Value))
                {
                    Source = Property,
                    Mode = BindingMode.OneWay
                });
        }

        public static async Task<ImageSource> GetBackground()
        {
            var vm = ServiceProvider.Get<VideoSourcesViewModel>();

            IBitmapImage bmp;

            switch (vm.SelectedVideoSourceKind?.Source)
            {
                case NoVideoItem _:
                    bmp = ScreenShot.Capture();
                    break;

                default:
                    var screenShotModel = ServiceProvider.Get<ScreenShotModel>();
                    bmp = await screenShotModel.GetScreenShot(vm.SelectedVideoSourceKind, true);
                    break;
            }

            if (bmp == null)
            {
                bmp = ScreenShot.Capture();
            }

            using (bmp)
            {
                var stream = new MemoryStream();
                bmp.Save(stream, ImageFormats.Png);

                stream.Seek(0, SeekOrigin.Begin);

                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                return decoder.Frames[0];
            }
        }
    }
}
