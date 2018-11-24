using System;
using System.Drawing;
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

        public static async Task UploadImage(this Bitmap Bitmap)
        {
            var uploadWriter = ServiceProvider.Get<ImageUploadWriter>();

            var shotVm = ServiceProvider.Get<ScreenShotViewModel>();

            var response = await uploadWriter.Save(Bitmap, shotVm.SelectedScreenShotImageFormat);

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