using System;
using System.Collections.Generic;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null) => null;

        public IScreen PickScreen() => null;

        public Rectangle? PickRegion() => null;
    }
}