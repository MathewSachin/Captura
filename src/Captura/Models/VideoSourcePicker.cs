using Screna;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow()
        {
            var picker = new SourcePicker();

            picker.ShowDialog();

            return picker.SelectedWindow;
        }
    }
}