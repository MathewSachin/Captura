using System;
using System.Collections.Generic;
using System.Drawing;

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

        public Rectangle? PickRegion()
        {
            return RegionPickerWindow.PickRegion();
        }
    }
}