using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null) => null;

        public IScreen PickScreen() => null;
    }
}