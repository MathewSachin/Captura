using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Captura.Models
{
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null) => null;

        public Screen PickScreen() => null;
    }
}