using System.Threading.Tasks;

namespace Captura.Models
{
    public interface IImageWriterItem
    {
        Task Save(IBitmapImage Image, ImageFormats Format, string FileName);

        string Display { get; }

        bool Active { get; set; }
    }
}
