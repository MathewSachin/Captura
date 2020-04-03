using System;
using DSize = System.Drawing.Size;
using WSize = System.Windows.Size;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Loc;
using Captura.Models;

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
                    var loc = ServiceProvider.Get<ILocalizationProvider>();
                    ServiceProvider.MessageProvider.ShowException(ex, loc.ImageUploadFailed);
                    break;

                case UploadResult uploadResult:
                    uploadResult.Url.WriteToClipboard();
                    break;
            }
        }

        public static DSize ToDrawingSize(this WSize Size)
        {
            return new DSize((int)Math.Round(Size.Width), (int)Math.Round(Size.Height));
        }

        public static WSize ToWpfSize(this DSize Size)
        {
            return new WSize(Size.Width, Size.Height);
        }
    }
}