using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow() => null;

        public Screen PickScreen() => null;
    }
}