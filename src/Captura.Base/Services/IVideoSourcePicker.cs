using System;
using System.Drawing;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        IWindow PickWindow(Predicate<IWindow> Filter = null);

        IScreen PickScreen();

        Rectangle? PickRegion();
    }
}