using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            return VideoSourcePickerWindow.PickWindow(SkipWindows);
        }

        public Screen PickScreen()
        {
            return VideoSourcePickerWindow.PickScreen();
        }
    }
}