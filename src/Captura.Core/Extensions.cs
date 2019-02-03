using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
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

        public static async Task UploadImage(this IBitmapImage Bitmap)
        {
            var uploadWriter = ServiceProvider.Get<ImageUploadWriter>();

            var settings = ServiceProvider.Get<Settings>();

            var response = await uploadWriter.Save(Bitmap, settings.ScreenShots.ImageFormat);

            switch (response)
            {
                case Exception ex:
                    var loc = ServiceProvider.Get<LanguageManager>();
                    ServiceProvider.MessageProvider.ShowException(ex, loc.ImageUploadFailed);
                    break;

                case UploadResult uploadResult:
                    uploadResult.Url.WriteToClipboard();
                    break;
            }
        }
    }
}