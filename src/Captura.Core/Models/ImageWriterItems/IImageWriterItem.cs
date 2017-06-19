using Captura.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public interface IImageWriterItem
    {
        void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents);
    }
}
