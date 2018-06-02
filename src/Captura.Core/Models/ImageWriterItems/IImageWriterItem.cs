using Captura.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Captura.Models
{
    public interface IImageWriterItem
    {
        Task Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents);

        string Display { get; }

        bool Active { get; set; }
    }
}
