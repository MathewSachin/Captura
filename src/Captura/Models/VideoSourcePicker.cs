using System;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
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