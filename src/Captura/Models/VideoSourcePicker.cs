using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Captura.Models;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            return VideoSourcePickerWindow.PickWindow(SkipWindows);
        }

        public Screen PickScreen()
        {
            return VideoSourcePickerWindow.PickScreen();
        }
    }
}