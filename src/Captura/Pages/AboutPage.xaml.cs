using System.Windows;
using Captura.Views;
using Microsoft.Win32;

namespace Captura
{
    public partial class AboutPage
    {
        void ViewLicenses(object Sender, RoutedEventArgs E)
        {
            LicensesWindow.ShowInstance();
        }

        void ViewCrashLogs(object Sender, RoutedEventArgs E)
        {
            CrashLogsWindow.ShowInstance();
        }

        void OpenImageEditor(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }

        void OpenAudioVideoTrimmer(object Sender, RoutedEventArgs E)
        {
            new TrimmerWindow().ShowAndFocus();
        }

        void OpenImageCropper(object Sender, RoutedEventArgs E)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.wmp;*.tiff",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                new CropWindow(ofd.FileName).ShowAndFocus();
            }
        }

        async void UploadToImgur(object Sender, RoutedEventArgs E)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.wmp;*.tiff",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                var imgSystem = ServiceProvider.Get<IImagingSystem>();

                using (var img = imgSystem.LoadBitmap(ofd.FileName))
                {
                    await img.UploadImage();
                }
            }
        }
    }
}
