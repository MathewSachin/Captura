using Captura.Models;

namespace Captura.Fakes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FakeWindowProvider : IMainWindow
    {
        public bool IsVisible
        {
            get => true;
            set { }
        }

        public bool IsMinimized
        {
            get => false;
            set { }
        }

        public void EditImage(string FileName) { }

        public void CropImage(string FileName) { }

        public void TrimMedia(string FileName) { }

        public void UploadToYouTube(string FileName) { }
    }
}
