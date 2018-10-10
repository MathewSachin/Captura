using Screna;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            if (SkipWindows != null)
            {
                return new Window(SkipWindows.First());
            }

            return null;
        }

        public IScreen PickScreen() => null;

        public Rectangle? PickRegion() => null;
    }
}