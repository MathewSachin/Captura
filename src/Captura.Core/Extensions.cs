using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Captura.ViewModels;
using Screna;

namespace Captura
{
    public static class Extensions
    {
        public static void ExecuteIfCan(this ICommand Command)
        {
            if (Command.CanExecute(null))
                Command.Execute(null);
        }

        static Bitmap Resize(this Bitmap Image, Size Resize, bool KeepAspectRatio, bool DisposeOriginal = true)
        {
            var resizeWidth = Resize.Width;
            var resizeHeight = Resize.Height;

            if (KeepAspectRatio)
            {
                var ratio = Math.Min((double) Resize.Width / Image.Width, (double) Resize.Height / Image.Height);

                resizeWidth = (int)(Image.Width * ratio);
                resizeHeight = (int)(Image.Height * ratio);
            }

            var resized = new Bitmap(Resize.Width, Resize.Height);

            using (var g = Graphics.FromImage(resized))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                try
                {
                    g.DrawImage(Image, 0, 0, resizeWidth, resizeHeight);
                }
                finally
                {
                    if (DisposeOriginal)
                        Image.Dispose();
                }
            }

            return resized;
        }

        public static async Task UploadImage(this Bitmap Bitmap)
        {
            var uploadWriter = ServiceProvider.Get<ImageUploadWriter>();

            var shotVm = ServiceProvider.Get<ScreenShotViewModel>();

            var response = await uploadWriter.Save(Bitmap, shotVm.SelectedScreenShotImageFormat);

            switch (response)
            {
                case Exception ex:
                    ServiceProvider.MessageProvider.ShowException(ex, "Image Upload failed");
                    break;

                case UploadResult uploadResult:
                    uploadResult.Url.WriteToClipboard();
                    break;
            }
        }
    }
}