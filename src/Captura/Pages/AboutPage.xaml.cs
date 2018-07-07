using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using Captura.Models;
using Captura.Views;
using Microsoft.Win32;
using Screna;

namespace Captura
{
    public partial class AboutPage
    {
        void ViewLicenses(object Sender, RoutedEventArgs E)
        {
            LicensesWindow.ShowInstance();
        }

        void Translate(object Sender, RoutedEventArgs E)
        {
            TranslationWindow.ShowInstance();
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
                var img = (Bitmap)Image.FromFile(ofd.FileName);

                var imgur = ServiceProvider.Get<ImgurWriter>();

                var response = await imgur.Save(img, ImageFormat.Png);

                switch (response)
                {
                    case Exception ex:
                        ServiceProvider.MessageProvider.ShowException(ex, "Upload to Imgur failed");
                        break;

                    case ImgurUploadResponse uploadResponse:
                        uploadResponse.Data.Link.WriteToClipboard();
                        break;
                }
            }
        }
    }
}
