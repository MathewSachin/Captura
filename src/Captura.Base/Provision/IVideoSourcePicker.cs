using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null);

        Screen PickScreen();
    }
}