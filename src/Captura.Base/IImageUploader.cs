using System;
using System.Threading.Tasks;

namespace Captura.Models
{
    public interface IImageUploader
    {
        Task<UploadResult> Upload(IBitmapImage Image, ImageFormats Format, Action<int> Progress);

        Task DeleteUploadedFile(string DeleteHash);

        string UploadServiceName { get; }
    }
}