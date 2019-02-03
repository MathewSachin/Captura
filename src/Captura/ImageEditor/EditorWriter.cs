using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EditorWriter : NotifyPropertyChanged, IImageWriterItem
    {
        public Task Save(IBitmapImage Image, ImageFormats Format, string FileName)
        {
            using (var stream = new MemoryStream())
            {
                Image.Save(stream, ImageFormats.Png);

                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                var win = new ImageEditorWindow();

                win.Open(decoder.Frames[0]);

                win.Show();
            }

            return Task.CompletedTask;
        }

        public string Display => "Editor";

        bool _active;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                OnPropertyChanged();
            }
        }

        public override string ToString() => Display;
    }
}
