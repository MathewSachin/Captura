using System;
using System.Drawing;

namespace Captura.Video
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
            return ScreenPickerWindow.PickScreen();
        }

        public Rectangle? PickRegion()
        {
            return RegionPickerWindow.PickRegion();
        }
    }
}