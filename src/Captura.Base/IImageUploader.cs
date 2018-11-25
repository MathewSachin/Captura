using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Captura.Models
{
    public interface IImageUploader
    {
        Task<UploadResult> Upload(Bitmap Image, ImageFormat Format, Action<int> Progress);

        Task DeleteUploadedFile(string DeleteHash);

        string UploadServiceName { get; }
    }
}