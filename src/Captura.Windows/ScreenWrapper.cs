using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Captura.Models;

namespace Captura
{
    class ScreenWrapper : IScreen
    {
        readonly Screen _screen;

        ScreenWrapper(Screen Screen)
        {
            _screen = Screen;
        }

        public Rectangle Rectangle => _screen.Bounds;

        public string DeviceName => _screen.DeviceName;

        public static IEnumerable<IScreen> Enumerate() => Screen.AllScreens.Select(M => new ScreenWrapper(M));
    }
}
