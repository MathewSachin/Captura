using System;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public IWindow PickWindow(Predicate<IWindow> Filter = null)
        {
            return VideoSourcePickerWindow.PickWindow(Filter);
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