using System.Drawing;
using System.Windows.Forms;
using Captura.Models;

namespace Captura
{
    public partial class VideoSourcePickerWindow
    {
        class ScreenWrapper : IScreen
        {
            readonly Screen _screen;

            public ScreenWrapper(Screen Screen)
            {
                _screen = Screen;
            }

            public Rectangle Rectangle => _screen.Bounds;

            public string DeviceName => _screen.DeviceName;
        }
    }
}
