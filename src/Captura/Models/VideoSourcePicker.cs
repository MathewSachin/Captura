using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            return VideoSourcePickerWindow.PickWindow(SkipWindows);
        }

        public IScreen PickScreen()
        {
            return VideoSourcePickerWindow.PickScreen();
        }
    }
}