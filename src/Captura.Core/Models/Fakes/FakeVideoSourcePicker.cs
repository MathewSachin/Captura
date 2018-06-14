using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow(IEnumerable<IntPtr> SkipWindows = null) => null;

        public Screen PickScreen() => null;
    }
}