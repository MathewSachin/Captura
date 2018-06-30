using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null);

        IScreen PickScreen();
    }
}