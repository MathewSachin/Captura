using System.Windows;
using Captura.Views;
using Microsoft.Win32;

namespace Captura
{
    public partial class AboutPage
    {
        void OpenAudioVideoTrimmer(object Sender, RoutedEventArgs E)
        {
            new TrimmerWindow().ShowAndFocus();
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

                using var img = imgSystem.LoadBitmap(ofd.FileName);
                await img.UploadImage();
            }
        }
    }
}
