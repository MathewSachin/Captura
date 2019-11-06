namespace Captura.Models
{
    public interface IMainWindow
    {
        bool IsVisible { get; set; }

        bool IsMinimized { get; set; }

        void EditImage(string FileName);

        void TrimMedia(string FileName);

        void UploadToYouTube(string FileName);
    }
}
