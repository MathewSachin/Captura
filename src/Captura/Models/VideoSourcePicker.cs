using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow()
        {
            var picker = new WindowPicker();

            picker.ShowDialog();

            return picker.SelectedWindow;
        }

        public Screen PickScreen()
        {
            var picker = new ScreenPicker();

            picker.ShowDialog();

            return picker.SelectedScreen;
        }
    }
}