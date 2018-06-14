using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        Window PickWindow(IEnumerable<IntPtr> SkipWindows = null);

        Screen PickScreen();
    }
}