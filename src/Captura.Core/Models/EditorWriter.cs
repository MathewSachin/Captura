using System.IO;
using System.Threading.Tasks;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EditorWriter : NotifyPropertyChanged, IImageWriterItem
    {
        public Task Save(IBitmapImage Image, ImageFormats Format, string FileName)
        {
            if (!File.Exists(FileName))
            {
                Image.Save(FileName, Format);
            }

            var winserv = ServiceProvider.Get<IMainWindow>();
            winserv.EditImage(FileName);

            return Task.CompletedTask;
        }

        public string Display => "Editor";

        bool _active;

        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }

        public override string ToString() => Display;
    }
}