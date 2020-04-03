using System;
using System.Drawing;

namespace Captura.Video
{
    public interface IVideoSourcePicker
    {
        IWindow PickWindow(Predicate<IWindow> Filter = null);

        IScreen PickScreen();

        Rectangle? PickRegion();
    }
}