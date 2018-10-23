using System;
using System.Collections.Generic;
using System.Drawing;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null);

        IScreen PickScreen();

        Rectangle? PickRegion();
    }
}